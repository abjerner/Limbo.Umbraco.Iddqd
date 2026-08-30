import { IddqdAuth } from "@limbo/iddqd/auth";

async function hi(url, config) {

    if (!config) config = {};
    if (!config.method) config.method = "GET";
    if (!config.headers) config.headers = {};

    const token = await IddqdAuth.TOKEN();
    config.headers.Authorization = `Bearer ${token}`;

    const res = await fetch(url, config);

    const contentType = res.headers.get("content-type") || "";

    if (contentType.includes("application/json")) {
        res.data = await res.json();
    } else if (contentType.startsWith("text/")) {
        res.textContent = await res.text();
    } else {
        throw new Error(`Unsupported content type: ${contentType}`);
    }

    if (!res.ok) {
        throw res;
    }

    return res;

}

async function get(url) {
    return await hi(url);
}

async function patch(url, config) {
    if (!config) config = {};
    config.method = "PATCH";
    return await hi(url, config);
}

async function patchJson(url, body, config) {
    if (!config) config = {};
    if (!config.headers) config.headers = {};
    config.headers["Content-Type"] = "application/json";
    config.body = JSON.stringify(body);
    return await patch(url, config);
}

async function put(url, config) {
    if (!config) config = {};
    config.method = "PUT";
    return await hi(url, config);
}

async function putJson(url, body, config) {
    if (!config) config = {};
    if (!config.headers) config.headers = {};
    config.headers["Content-Type"] = "application/json";
    config.body = JSON.stringify(body);
    return await put(url, config);
}

async function post(url, config) {
    if (!config) config = {};
    config.method = "POST";
    return await hi(url, config);
}

async function _delete(url, config) {
    if (!config) config = {};
    config.method = "DELETE";
    return await hi(url, config);
}


async function getData(url, config) {
    const response = await hi(url, config);
    return response.data;
}

async function postData(url, config) {
    const response = await post(url, config);
    return response.data;
}

const baseUrl = "/umbraco/management/api/v1/limbo/iddqd";

const domains = {
    getAll: async () => await getData(`${baseUrl}/domains`)
};

const media = {
    getMedia: async (key) => await getData(`${baseUrl}/media/${key}`),
    getMediaExamine: async (key) => await getData(`${baseUrl}/media/${key}/examine`),
    getMediaType: async (key) => await getData(`${baseUrl}/media-types/${key}`),
};

const packages = {
    getManifests: async () => await getData(`${baseUrl}/packages/manifests`)
};

export class IddqdService {

    static domains = domains;
    static media = media;
    static packages = packages;

    static assemblies = {
        getLinks(assembly) {
            return [
                { name: "Website", icon: "icon-globe", url: assembly.packageProjectUrl },
                { name: "Documentation", icon: "icon-book", url: assembly.documentationUrl },
                { name: "Repository", icon: assembly.repositoryUrl?.indexOf("github.com") !== -1 ? "icon-github" : "icon-forking", url: assembly.repositoryUrl },
                { name: "Umbraco Marketplace", icon: "icon-store", url: assembly.marketplaceUrl },
                { name: "NuGet", icon: "icon-iddqd-nuget", url: assembly.nuGetUrl }
            ];
        },
        updateLinks(assembly) {
            if (!assembly) return;
            assembly.links = IddqdService.assemblies.getLinks(assembly)
        }
    };

    static content = {
        get: async (key) => await getData(`${baseUrl}/content/${key}`),
        getExamine: async (key) => await getData(`${baseUrl}/content/${key}/examine`),
        updateExamine: async (key) => await postData(`${baseUrl}/content/${key}/examine`)
    };

    static dataTypes = {
        get: async function (key) {

            const dataType = await getData(`${baseUrl}/data-types/${key}`);

            IddqdService.assemblies.updateLinks(dataType?.editor?.assembly);
            if (dataType.createDate) dataType.createDate = new Date(dataType.createDate);
            if (dataType.updateDate) dataType.updateDate = new Date(dataType.updateDate);

            return dataType;

        },
        getRelations: async (key) => await getData(`${baseUrl}/data-types/${key}/relations`),
        getAll: async () => await getData(`${baseUrl}/data-types`)
    };

    static members = {
        get: async function (key) {
            const response = await get(`${baseUrl}/members/${key}`);
            if (response.data.createDate) response.data.createDate = new Date(response.data.createDate);
            if (response.data.updateDate) response.data.updateDate = new Date(response.data.updateDate);
            return response.data;
        },
        getExamine: async (key) => await getData(`${baseUrl}/members/${key}/examine`),
        updateExamine: async (key) => await postData(`${baseUrl}/members/${key}/examine`)
    };

    static memberTypes = {
        get: async function (key) {
            return await getData(`${baseUrl}/member-types/${key}`);
        }
    };

    static async getContentType(key) {
        const response = await get(`${baseUrl}/content-types/${key}`);
        if (response.data.createDate) response.data.createDate = new Date(response.data.createDate);
        if (response.data.updateDate) response.data.updateDate = new Date(response.data.updateDate);
        return response.data;
    }

    static async getDataType(key) {
        const response = await get(`${baseUrl}/data-types/${key}`);
        IddqdService.assemblies.updateLinks(response.data?.editor?.assembly);
        if (response.data.createDate) response.data.createDate = new Date(response.data.createDate);
        if (response.data.updateDate) response.data.updateDate = new Date(response.data.updateDate);
        return response.data;
    }

    static async getMemberType(key) {
        const response = await get(`${baseUrl}/member-types/${key}`);
        if (response.data.createDate) response.data.createDate = new Date(response.data.createDate);
        if (response.data.updateDate) response.data.updateDate = new Date(response.data.updateDate);
        return response.data;
    }

    static async getUser(key) {
        const response = await get(`${baseUrl}/users/${key}`);
        if (response.data.createDate) response.data.createDate = new Date(response.data.createDate);
        if (response.data.updateDate) response.data.updateDate = new Date(response.data.updateDate);
        return response.data;
    }

    static async getServerVariables() {
        return await getData(`${baseUrl}/serverVariables`);
    }

}

export default IddqdService;
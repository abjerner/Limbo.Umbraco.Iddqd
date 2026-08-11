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

export class IddqdHttpClient {

    static async get(url) {
        return await hi(url);
    }

    static async patch(url, config) {
        if (!config) config = {};
        config.method = "PATCH";
        return await hi(url, config);
    }

    static async patchJson(url, body, config) {
        if (!config) config = {};
        if (!config.headers) config.headers = {};
        config.headers["Content-Type"] = "application/json";
        config.body = JSON.stringify(body);
        return await patch(url, config);
    }

    static async post(url, config) {
        if (!config) config = {};
        config.method = "POST";
        return await hi(url, config);
    }

    static async put(url, config) {
        if (!config) config = {};
        config.method = "PUT";
        return await hi(url, config);
    }

    static async putJson(url, body, config) {
        if (!config) config = {};
        if (!config.headers) config.headers = {};
        config.headers["Content-Type"] = "application/json";
        config.body = JSON.stringify(body);
        return await put(url, config);
    }

    static async post(url, config) {
        if (!config) config = {};
        config.method = "POST";
        return await hi(url, config);
    }

    static async _delete(url, config) {
        if (!config) config = {};
        config.method = "DELETE";
        return await hi(url, config);
    }

};

export default IddqdHttpClient;
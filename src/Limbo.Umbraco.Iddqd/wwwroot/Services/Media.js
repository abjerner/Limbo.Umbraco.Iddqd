import { IddqdAuth } from "@limbo/iddqd/auth";
import { IddqdHttpClient as http } from "@limbo/iddqd/http";

const baseUrl = "/umbraco/management/api/v1/limbo/iddqd";

export class IddqdMediaService {

    static async get(key) {

        const response = await http.get(`${baseUrl}/media/${key}`);

        response.data.createDate = new Date(response.data.createDate);
        response.data.updateDate = new Date(response.data.updateDate);

        return response.data;

    }

    static async getExamine(key) {
        return (await http.get(`${baseUrl}/media/${key}/examine`)).data;
    }

    static async updateExamine(key, indexName) {
        return (await http.post(`${baseUrl}/media/${key}/examine/indexes/${indexName}`)).data;
    }


};
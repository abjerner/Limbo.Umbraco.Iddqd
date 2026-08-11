import { IddqdAuth } from "@limbo/iddqd/auth";
import { IddqdHttpClient as http } from "@limbo/iddqd/http";

const baseUrl = "/umbraco/management/api/v1/limbo/iddqd";

export class IddqdMediaTypesService {

    static async get(key) {

        const response = await http.get(`${baseUrl}/media-types/${key}`);

        if (response.data.createDate) response.data.createDate = new Date(response.data.createDate);
        if (response.data.updateDate) response.data.updateDate = new Date(response.data.updateDate);

        return response.data;

    }

};

export default IddqdMediaTypesService;
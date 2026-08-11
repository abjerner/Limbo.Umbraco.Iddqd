import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { IddqdAuth } from "@limbo/iddqd/auth";
import { IddqdPackage } from "@limbo/iddqd/package";
import { IddqdService } from "@limbo/iddqd/service";

export const onInit = (_host, extensionRegistry) => {

    _host.consumeContext(UMB_AUTH_CONTEXT, async (authContext) => {

        const config = authContext.getOpenApiConfiguration();
        IddqdAuth.TOKEN = config.token;

        IddqdPackage.serverVariables = await IddqdService.getServerVariables();

    });

};
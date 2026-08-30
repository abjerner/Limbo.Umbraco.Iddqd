import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";

import {
    umbExtensionsRegistry
} from "@umbraco-cms/backoffice/extension-registry";

// import {
//     umbOpenModal
// } from "@umbraco-cms/backoffice/modal";

// import {
//     EXTENSION_DETAILS_MODAL
// } from "./extension-details-modal.token.js";


export default class ExtensionInsightsElement extends UmbLitElement {

    static properties = {
        extensions: { state: true },
        filter: { state: true },
        typeFilter: { state: true }
    };

    constructor() {
        super();
    }

    provideContext() {
        // You can leave this empty for now
    }

    render() {
        return html`hello there!`;
    }

}

customElements.define("iddqd-extensions-workspace-view", ExtensionInsightsElement);
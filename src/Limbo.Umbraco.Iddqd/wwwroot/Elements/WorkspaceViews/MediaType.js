import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";

//import { getDisplayStateFromUserStatus } from "@umbraco-cms/backoffice/user";

import { UMB_ENTITY_WORKSPACE_CONTEXT, UMB_ROUTABLE_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/workspace";

import { LimboIddqdWorkspaceViewBaseElement } from "@limbo/iddqd/elements/workspace-views/base";

import { IddqdMediaService } from "@limbo/iddqd/services/media";

import { delayed } from "@limbo/iddqd/package";

export class LimboIddqdMediaWorkspaceViewElement extends LimboIddqdWorkspaceViewBaseElement {

    constructor() {
        super();
    }

    async init(type, key) {
        if (type !== "media") return;
        this.media = await IddqdMediaService.get(key);
        this.examine = await IddqdMediaService.getExamine(key);
        this.requestUpdate();
    }

    async refreshIndex(index) {

        index.loading = true;
        index.refreshButtonState = "waiting";
        this.requestUpdate();

        this.examine = await delayed(() => IddqdMediaService.getExamine(this.media.key));

        index.loading = false;
        this.requestUpdate();

    }

    async updateIndex(index) {

        index.loading = true;
        index.updateButtonState = "waiting";
        this.requestUpdate();

        this.examine = await delayed(() => IddqdMediaService.updateExamine(this.media.key, index.indexName));

        index.loading = false;
        this.requestUpdate();

    }

    render() {
        return html`
            <div>
                ${when(this.media, () => html`
                     ${this.renderDetails()}
                     ${this.renderExamine()}
                `)}
            </div>
        `;
    }

    renderDetails() {
        return html`
            <uui-box headline="Media">
                <uui-box-body>
                    <table class="details">
                        ${this.renderDetail("ID", this.media.id, "id")}
                        ${this.renderDetail("Key", this.media.key, "key")}
                        ${this.renderDetail("Name", this.media.name, "name")}
                        ${this.renderDetail("Created", this.media.createDate)}
                        ${this.renderDetail("Updated", this.media.updateDate)}
                    </table>
                </uui-box-body>
            </uui-box>
        `;
    }

    renderExamine() {

        return html`
            ${when(this.examine?.length > 0, () => html`
                ${repeat(this.examine, (index) => html`
                    <uui-box headline="Examine: ${index.indexName}" class="index ${index.loading ? 'loading' : ''}">
                        <div slot="header-actions">
                            <uui-button look="primary" colorf="positive" label="Refresh" @click=${() => this.refreshIndex(index)} .state=${index.refreshButtonState}></uui-button>
                            <uui-button look="primary" color="positive" label="Re-index" @click=${() => this.updateIndex(index)} .state=${index.updateButtonState}></uui-button>
                        </div>
                        <uui-box-body>
                            ${when(!index.result, () => html`
                                <p>Not found in index <strong>${index.indexName}</strong>.</p>
                            `)}
                            ${when(index.result, () => html`
                                <table class="details">
                                    ${repeat(Object.keys(index.result?.allValues ?? {}), (key) => html`
                                        <tr>
                                            <th>${key}</th>
                                            <td>${index.result.allValues[key]}</td>
                                        </tr>
                                    `)}
                                </table>
                            `)}
                        </uui-box-body>
                    </uui-box>
                `)}
            `)}
        `;

    }

}

customElements.define("limbo-iddqd-media-workspace-view", LimboIddqdMediaWorkspaceViewElement);

export default LimboIddqdMediaWorkspaceViewElement;
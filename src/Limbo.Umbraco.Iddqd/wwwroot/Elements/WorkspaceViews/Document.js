import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";

//import { getDisplayStateFromUserStatus } from "@umbraco-cms/backoffice/user";

import { UMB_ENTITY_WORKSPACE_CONTEXT, UMB_ROUTABLE_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/workspace";

import { IddqdContentService } from "@limbo/iddqd/services/content";




import { LimboIddqdWorkspaceViewBaseElement } from "./Base.js";





const DELAY = 250;

async function delay(ms) {
    return new Promise((resolve) => setTimeout(resolve, ms));
}

async function delayed(callback) {
    const [, result] = await Promise.all([delay(DELAY), callback()]);
    return result;
}

export class LimboIddqdContentWorkspaceViewElement extends LimboIddqdWorkspaceViewBaseElement {

    async init(type, key) {

        if (type != "document") return;

        this.content = await IddqdContentService.get(key);
        this.examine = await IddqdContentService.getExamine(key);
        this.requestUpdate();

    }

    async refreshIndex(index) {

        index.loading = true;
        index.refreshButtonState = "waiting";
        this.requestUpdate();

        this.examine = await delayed(() => IddqdContentService.getExamine(this.content.key));

        index.loading = false;
        this.requestUpdate();

    }

    async updateIndex(index) {

        index.loading = true;
        index.updateButtonState = "waiting";
        this.requestUpdate();

        this.examine = await delayed(() => IddqdContentService.updateExamine(this.content.key, index.indexName));

        index.loading = false;
        this.requestUpdate();

    }

    render() {
        return html`
            <div>
                ${when(this.content, () => html`
                     ${this.renderDetails()}
                     ${this.renderExamine()}
                `)}
            </div>
        `;
    }

    renderDetails() {

        return html`
            <uui-box headline="Content">
                <uui-box-body>
                    <table class="details">
                        <tr>
                            <th>ID</th>
                            <td>
                                <span class="select-all">${this.content.id}</span>
                            </td>
                        </tr>
                        <tr>
                            <th>Key</th>
                            <td>
                                <span class="select-all">${this.content.key}</span>
                            </td>
                        </tr>
                        <tr>
                            <th>Name</th>
                            <td>${this.content.name}</td>
                        </tr>
                        <tr>
                            <th>Created</th>
                            <td>
                                ${this.date(this.content.createDate)}
                                <small>(${this.fromNow(this.content.createDate)})</small>
                            </td>
                        </tr>
                        <tr>
                            <th>Updated</th>
                            <td>
                                ${this.date(this.content.updateDate)}
                                <small>(${this.fromNow(this.content.updateDate)})</small>
                            </td>
                        </tr>
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

    static styles = css`

        ${LimboIddqdWorkspaceViewBaseElement.styles}

        .index.loading {
            opacity: 0.4;
        }

        .index th, .index td {
            vertical-align: top;
        }

    `;

}

customElements.define("limbo-iddqd-content-workspace-view", LimboIddqdContentWorkspaceViewElement);

export default LimboIddqdContentWorkspaceViewElement;
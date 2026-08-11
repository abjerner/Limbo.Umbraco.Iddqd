import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";

//import { getDisplayStateFromUserStatus } from "@umbraco-cms/backoffice/user";

import { UMB_ENTITY_WORKSPACE_CONTEXT, UMB_ROUTABLE_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/workspace";

import { IddqdService } from "@limbo/iddqd/service";

import { LimboIddqdWorkspaceViewBaseElement } from "./Base.js";

export class LimboIddqdContentTypeWorkspaceViewElement extends LimboIddqdWorkspaceViewBaseElement {

    constructor() {
        super();
    }

    async init(type, key) {
        if (type !== "document-type") return;
        this.contentType = await IddqdService.getContentType(key);
        this.requestUpdate();
    }

    render() {
        return html`
            <div>
                ${when(this.contentType, () => html`
                     ${this.renderDetails()}
                     ${this.renderProperties()}
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
                            <td>${this.contentType.id}</td>
                        </tr>
                        <tr>
                            <th>Key</th>
                            <td>${this.contentType.key}</td>
                        </tr>
                        <tr>
                            <th>Alias</th>
                            <td>${this.contentType.alias}</td>
                        </tr>
                        <tr>
                            <th>Icon</th>
                            <td>${this.contentType.icon}</td>
                        </tr>
                        <tr>
                            <th>Name</th>
                            <td>${this.contentType.name}</td>
                        </tr>
                        <tr>
                            <th>Created</th>
                            <td>
                                ${this.date(this.contentType.createDate)}
                                <small>(${this.fromNow(this.contentType.createDate)})</small>
                            </td>
                        </tr>
                        <tr>
                            <th>Updated</th>
                            <td>
                                ${this.date(this.contentType.updateDate)}
                                <small>(${this.fromNow(this.contentType.updateDate)})</small>
                            </td>
                        </tr>
                    </table>
                </uui-box-body>
            </uui-box>
        `;
    }

    renderProperties() {

        return html`
            <uui-box headline="Tabs, Groups & Properties">
                <uui-box-body>
                    <table class="list">
                        <thead>
                            <tr>
                                <th>ID / Key</th>
                                <th>Name / Alias</th>
                                <th>Data Type</th>
                                <th>Value Type</th>
                                <th>Order</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${repeat(this.contentType.propertyGroups, (g) => html`
                                <tr>
                                    <td>
                                        ${g.id}<br />
                                        <small class="muted">${g.key}</small>
                                    </td>
                                    <td>
                                        <strong>${g.name}</strong> (${g.type})<br />
                                        <small class="muted">${g.alias}</small>
                                    </td>
                                    <td></td>
                                    <td></td>
                                    <td>${g.sortOrder}</td>
                                </tr>
                                ${repeat(g.propertyTypes, (p) => html`
                                    <tr>
                                        <td>
                                            ${p.id}<br />
                                            <small class="muted">${p.key}</small>
                                        </td>
                                        <td>
                                            <strong>${p.name}</strong><br />
                                            <small class="muted">${p.alias}</small>
                                        </td>
                                        <td>
                                            <strong>${p.dataType?.name}</strong><br />
                                            <small class="muted">${p.dataType?.editorAlias} / ${p.dataType?.editorUiAlias}</small>
                                        </td>
                                        <td>
                                            <strong>${p.clrType.name}</strong><br />
                                            <small class="muted">
                                                ${p.clrType.assembly}.dll
                                                /
                                                ${p.clrType.namespace}
                                            </small>
                                        </td>
                                        <td>${p.sortOrder}</td>
                                    </tr>
                                `)}
                            `)}
                        </tbody>
                    </table>
                </uui-box-body>
            </uui-box>
        `;

    }

    static styles = css`

        ${LimboIddqdWorkspaceViewBaseElement.styles}

    `;

}

customElements.define("limbo-iddqd-content-type-workspace-view", LimboIddqdContentTypeWorkspaceViewElement);

export default LimboIddqdContentTypeWorkspaceViewElement;
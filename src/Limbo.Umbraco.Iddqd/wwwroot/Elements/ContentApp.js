import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";

import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from '@umbraco-cms/backoffice/document';

//import { getDisplayStateFromUserStatus } from "@umbraco-cms/backoffice/user";

import {
    UMB_ENTITY_WORKSPACE_CONTEXT,
    UMB_ROUTABLE_WORKSPACE_CONTEXT,
} from '@umbraco-cms/backoffice/workspace';


import { IddqdService } from "@limbo/iddqd/service";




const userStates = [
    { key: 'All', color: 'positive', look: 'secondary' },
    { key: 'Active', color: 'positive', look: 'primary' },
    { key: 'Disabled', color: 'danger', look: 'primary' },
    { key: 'LockedOut', color: 'danger', look: 'secondary' },
    { key: 'Invited', color: 'warning', look: 'primary' },
    { key: 'Inactive', color: 'warning', look: 'primary' },
];

export const getDisplayStateFromUserStatus = (status) =>
    userStates
        .filter((state) => state.key === status)
        .map((state) => ({
            ...state,
            key: 'state' + state.key,
        }))[0];







export class LimboIddqdContentAppElement extends UmbElementMixin(LitElement) {

    #workspaceContext;
    #entityWorkspace;

    constructor() {

        super();

        this.consumeContext(UMB_ENTITY_WORKSPACE_CONTEXT, (ctx) => {

            this.#entityWorkspace = ctx;
            if (!ctx) return;

            const type = ctx.getEntityType?.();
            const key = ctx.getUnique?.();

            if (type && key) this.init(type, key);

        });

    }

    init(type, key) {

        const self = this;

        if (type == "document") {

            console.log("Initializing for content with key", key);

            IddqdService.getContent(key).then(function (res) {
                self.content = res.data;
                self.requestUpdate();
            });

            IddqdService.getContentExamine(key).then(function (res) {
                self.examine = res.data;
                self.requestUpdate();
            });

        } else if (type == "document-type") {

            console.log("Initializing for content type with key", key);

            IddqdService.getContentType(key).then(function (res) {
                self.contentType = res.data;
                self.requestUpdate();
            });

        } else if (type == "data-type") {

            console.log("Initializing for data type with key", key);

            IddqdService.getDataType(key).then(function (res) {
                self.dataType = res.data;
                self.requestUpdate();
            });

            IddqdService.getDataTypeRelations(key).then(function (res) {
                self.relations = res.data;
                self.requestUpdate();
            });

        } else if (type == "user") {

            console.log("Initializing for user with key", key);

            IddqdService.getUser(key).then(function (res) {
                self.user = res.data;
                self.userDisplayState = self.user.userState ? getDisplayStateFromUserStatus(self.user.userState) : null;
                self.requestUpdate();
            });

        }

    }

    render() {
        return html`
            <div>
                ${when(this.content, () => this.renderContent())}
                ${when(this.contentType, () => this.renderContentType())}
                ${when(this.dataType, () => this.renderDataType())}
                ${when(this.user, () => this.renderUser())}
            </div>
        `;
    }

    renderContent() {

        const createDate = new Date(this.content.createDate);
        const updateDate = new Date(this.content.updateDate);

        const dateOptions = {
            day: "numeric",
            hour: "numeric",
            minute: "numeric",
            month: "long",
            second: "numeric",
            year: "numeric"
        };

        return html`
            <uui-box headline="Content">
                <uui-box-body>
                    <table class="details">
                        <tr>
                            <th>ID</th>
                            <td>${this.content.id}</td>
                        </tr>
                        <tr>
                            <th>Key</th>
                            <td>${this.content.key}</td>
                        </tr>
                        <tr>
                            <th>Name</th>
                            <td>${this.content.name}</td>
                        </tr>
                        <tr>
                            <th>Created</th>
                            <td>
                                ${this.localize.date(createDate, dateOptions)}
                            </td>
                        </tr>
                        <tr>
                            <th>Updated</th>
                            <td>
                                ${this.localize.date(updateDate, dateOptions)}
                            </td>
                        </tr>
                    </table>
                </uui-box-body>
            </uui-box>

            ${when(this.examine?.length > 0, () => html`
                ${repeat(this.examine, (index) => html`
                    <uui-box headline="Examine: ${index.indexName}">
                        <div slot="header-actions">
                            <uui-button label="Re-index"></uui-button>
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

    renderContentType() {

        const createDate = new Date(this.contentType.createDate);
        const updateDate = new Date(this.contentType.updateDate);

        const dateOptions = {
            day: "numeric",
            hour: "numeric",
            minute: "numeric",
            month: "long",
            second: "numeric",
            year: "numeric"
        };

        return html`
            <uui-box headline="Content Type">
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
                                ${this.localize.date(createDate, dateOptions)}
                                <small>(<redirects-from-now>${createDate}</redirects-from-now>)</small>
                            </td>
                        </tr>
                    </table>
                </uui-box-body>
            </uui-box>
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

    renderDataType() {

        const createDate = new Date(this.dataType.createDate);
        const updateDate = new Date(this.dataType.updateDate);

        const dateOptions = {
            day: "numeric",
            hour: "numeric",
            minute: "numeric",
            month: "long",
            second: "numeric",
            year: "numeric"
        };

        return html`
            <uui-box headline="Data Type">
                <uui-box-body>
                    <table class="details">
                        <tr>
                            <th>ID</th>
                            <td>${this.dataType.id}</td>
                        </tr>
                        <tr>
                            <th>Key</th>
                            <td>${this.dataType.key}</td>
                        </tr>
                        <tr>
                            <th>Name</th>
                            <td>${this.dataType.name}</td>
                        </tr>
                        <tr>
                            <th>Editor Alias</th>
                            <td>${this.dataType.editorAlias}</td>
                        </tr>
                        <tr>
                            <th>Editor UI Alias</th>
                            <td>${this.dataType.editorUiAlias}</td>
                        </tr>
                        <tr>
                            <th>Created</th>
                            <td>
                                ${this.localize.date(createDate, dateOptions)}
                                <small>(<redirects-from-nowww>${createDate}</redirects-from-nowww>)</small>
                            </td>
                        </tr>
                    </table>
                </uui-box-body>
            </uui-box>
            ${when(this.relations, () => html`
                <uui-box class="relations" headline="Relations">
                    <uui-box-body>
                        <div class="stack">
                            ${this.renderDataTypeRelationsContentTypes("Content Types", "Content Type", "No related content types found.", this.relations.contentTypes)}
                            ${this.renderDataTypeRelationsContentTypes("Media Types", "Media Type", "No related media types found.", this.relations.mediaTypes)}
                            ${this.renderDataTypeRelationsContentTypes("Member Types", "Member Type", "No related member types found.", this.relations.memberTypes)}
                        </div>
                    </uui-box-body>
                </uui-box>
            `)}
        `;

    }

    renderDataTypeRelationsContentTypes(plural, singular, empty, items) {

        if (!items) items = [];

        return html`
            <div>
                <h4>${plural}</h4>
                ${when(items.length == 0, () => html`<p>${empty}</p>`)}
                ${when(items.length > 0, () => html`
                    <table class="list border">
                        <thead>
                            <tr>
                                <th>${singular}</th>
                                <th>Property Type</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${repeat(items, (p) => html`
                                <tr>
                                    <td>
                                        <strong>${p.contentTypeName}</strong><br />
                                        <small class="muted">${p.contentTypeAlias}</small>
                                    </td>
                                    <td>
                                        <strong>${p.propertyName}</strong><br />
                                        <small class="muted">${p.propertyAlias}</small>
                                    </td>
                                </tr>
                            `)}
                        </tbody>
                    </table>
                `)}
            </div>
        `;

    }

    renderUser() {

        const createDate = new Date(this.user.createDate);
        const updateDate = new Date(this.user.updateDate);

        const dateOptions = {
            day: "numeric",
            hour: "numeric",
            minute: "numeric",
            month: "long",
            second: "numeric",
            year: "numeric"
        };

        return html`
            <uui-box headline="User">
                <uui-box-body>
                    <table class="details">
                        <tr>
                            <th>ID</th>
                            <td>${this.user.id}</td>
                        </tr>
                        <tr>
                            <th>Key</th>
                            <td>${this.user.key}</td>
                        </tr>
                        <tr>
                            <th>Username</th>
                            <td>${this.user.username}</td>
                        </tr>
                        <tr>
                            <th>Email</th>
                            <td>${this.user.email}</td>
                        </tr>
                        <tr>
                            <th>Name</th>
                            <td>${this.user.name}</td>
                        </tr>
                        <tr>
                            <th>State</th>
                            <td>
                                <div id="state" class="user-info-item">
				                    <uui-tag look="${this.userDisplayState?.look}" color="${this.userDisplayState?.color}">
					                    ${this.localize.term('user_' + this.userDisplayState?.key)}
				                    </uui-tag>
			                    </div>
                            </td>
                        </tr>
                        <tr>
                            <th>Language</th>
                            <td>
                                <code>${this.user.language}</code>
                            </td>
                        </tr>
                        <tr>
                            <th>Created</th>
                            <td>
                                ${this.localize.date(createDate, dateOptions)}
                                <small>(<redirects-from-now>${createDate}</redirects-from-now>)</small>
                            </td>
                        </tr>
                        <tr>
                            <th>Updated</th>
                            <td>
                                ${this.localize.date(updateDate, dateOptions)}
                                <small>(<redirects-from-now>${updateDate}</redirects-from-now>)</small>
                            </td>
                        </tr>
                    </table>
                    <pre>${JSON.stringify(this.user, null, 2)}</pre>
                </uui-box-body>
            </uui-box>
        `;

    }

    static styles = css`

        :host > div {
            padding: 20px;
        }

        .relations h4 {
            margin-top: 0;
        }

        .stack {
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        table.details {
            width: 100%;
            th {
                text-align: left;
                min-width: 200px;
            }
            td {
                width: 100%;
            }
            th, td {
                padding: 8px 10px 5px 10px;
            }
            tr + tr th,
            tr + tr td {
                border-top: 1px solid #e9e9eb;
            }
        }

        table.list {
            width: 100%;
            &.border {
                border: 1px solid #e9e9eb;
            }
            th {
                text-align: left;
            }
            th, td {
                padding: 8px 10px 5px 10px;
            }
            tbody {
                th, td {
                    border-top: 1px solid #e9e9eb;
                }
            }
        }

        uui-box + uui-box {
            margin-top: 20px;
        }

        .muted {
            color: #999;
        }

    `;

}

customElements.define("limbo-iddqd-content-app", LimboIddqdContentAppElement);

export default LimboIddqdContentAppElement;
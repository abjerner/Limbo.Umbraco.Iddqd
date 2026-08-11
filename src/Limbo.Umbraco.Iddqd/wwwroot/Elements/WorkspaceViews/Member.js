import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";

//import { getDisplayStateFromUserStatus } from "@umbraco-cms/backoffice/user";

import { UMB_ENTITY_WORKSPACE_CONTEXT, UMB_ROUTABLE_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/workspace";

import { LimboIddqdWorkspaceViewBaseElement } from "@limbo/iddqd/elements/workspace-views/base";


import { IddqdMembersService } from "@limbo/iddqd/services/members";

import { delayed } from "@limbo/iddqd/package";

export class LimboIddqdMemberWorkspaceViewElement extends LimboIddqdWorkspaceViewBaseElement {

    constructor() {
        super();
    }

    async init(type, key) {
        if (type !== "member") return;
        this.member = await IddqdMembersService.get(key);
        this.examine = await IddqdMembersService.getExamine(key);
        this.requestUpdate();
    }

    async refreshIndex(index) {

        index.loading = true;
        index.refreshButtonState = "waiting";
        this.requestUpdate();

        this.examine = await delayed(() => IddqdMembersService.getExamine(this.member.key));

        index.loading = false;
        this.requestUpdate();

    }

    async updateIndex(index) {

        index.loading = true;
        index.updateButtonState = "waiting";
        this.requestUpdate();

        this.examine = await delayed(() => IddqdMembersService.updateExamine(this.member.key, index.indexName));

        index.loading = false;
        this.requestUpdate();

    }

    render() {
        return html`
            <div>
                ${when(this.member, () => html`
                     ${this.renderDetails()}
                     ${this.renderExamine()}
                `)}
            </div>
        `;
    }

    renderStatus() {

        const temp = [];

        if (this.member.isApproved) {
            temp.push(html`<uui-tag look="primary" color="positive">Approved</uui-tag>`);
        } else {
            temp.push(html`<uui-tag look="primary" color="danger">Not approved</uui-tag>`);
        }

        if (this.member.isLockedOut) {
            temp.push(html`<uui-tag look="primary" color="danger">Locked out</uui-tag>`);
        }

        return html`<div style="display: flex; gap: 5px;">${temp}</div>`;

    }

    renderDetails() {
        return html`
            <uui-box headline="Member">
                <uui-box-body>
                    <table class="details">
                        ${this.renderDetail("ID", this.member.id, "id")}
                        ${this.renderDetail("Key", this.member.key, "key")}
                        ${this.renderDetail("Username", this.member.username, "username")}
                        ${this.renderDetail("Email", this.member.email, "email")}
                        ${this.renderDetail("Name", this.member.name, "name")}
                        ${this.renderDetail("Status", this.renderStatus())}
                        ${this.renderDetail("Created", this.member.createDate)}
                        ${this.renderDetail("Updated", this.member.updateDate)}
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

customElements.define("limbo-iddqd-member-workspace-view", LimboIddqdMemberWorkspaceViewElement);

export default LimboIddqdMemberWorkspaceViewElement;
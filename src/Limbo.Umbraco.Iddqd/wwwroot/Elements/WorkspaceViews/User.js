import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";

//import { getDisplayStateFromUserStatus } from "@umbraco-cms/backoffice/user";

import { UMB_ENTITY_WORKSPACE_CONTEXT, UMB_ROUTABLE_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/workspace";

import { IddqdService } from "@limbo/iddqd/service";

import { LimboIddqdWorkspaceViewBaseElement } from "./Base.js";

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

export class LimboIddqdUserWorkspaceViewElement extends LimboIddqdWorkspaceViewBaseElement {

    constructor() {
        super();
    }

    async init(type, key) {

        if (type !== "user") return;

        console.log("Initializing for user with key", key);

        this.user = await IddqdService.getUser(key);

        this.user.createDate = new Date(this.user.createDate);
        this.user.updateDate = new Date(this.user.updateDate);
        this.userDisplayState = this.user.userState ? getDisplayStateFromUserStatus(this.user.userState) : null;


        this.requestUpdate();

    }

    #renderDetails() {

        return html`
            <uui-box headline="User">
                <uui-box-body>
                    <table class="details">
                        <tr>
                            <th>ID</th>
                            <td><span class="select-all">${this.user.id}</span></td>
                        </tr>
                        <tr>
                            <th>Key</th>
                            <td><span class="select-all">${this.user.key}</span></td>
                        </tr>
                        <tr>
                            <th>Username</th>
                            <td><span class="select-all">${this.user.username}</span></td>
                        </tr>
                        <tr>
                            <th>Email</th>
                            <td><span class="select-all">${this.user.email}</span></td>
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
                                ${this.date(this.user.createDate)}
                                <small>(${this.fromNow(this.user.createDate)})</small>
                            </td>
                        </tr>
                        <tr>
                            <th>Updated</th>
                            <td>
                                ${this.date(this.user.updateDate)}
                                <small>(${this.fromNow(this.user.updateDate)})</small>
                            </td>
                        </tr>
                    </table>
                </uui-box-body>
            </uui-box>
        `;

    }

    #renderGroups() {

        return html`
            <uui-box headline="Groups">
                <table class="table list">
                    <thead>
                        <tr>
                            <th>ID / Key</th>
                            <td></td>
                            <th>Name / Alias</th>
                            <th>Sections</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${repeat(this.user.groups, (group) => html`
                            <tr>
                                <td>
                                    <span class="select-all">${group.id}</span><br />
                                    <small class="select-all">${group.key}</small>
                                </td>
                                <td>
                                    <uui-icon name=${group.icon}></uui-icon>
                                </td>
                                <td>
                                    <strong>${group.name}</strong><br />
                                    <small>${group.alias}</small>
                                </td>
                                <td>
                                ${when(group.allowedSections?.length > 0, () => html`
                                    ${group.allowedSections.join(", ")}
                                `, () => html`
                                    <em class="muted">N/A</em>
                                `)}
                                </td>
                            </tr>
                        `)}
                    </tbody>
                </table>
            </uui-box>
        `;

    }

    render() {
        return html`
            <div>
                ${when(this.user, () => html`
                     ${this.#renderDetails()}
                     ${this.#renderGroups()}
                `)}
            </div>
        `;
    }

    static styles = css`

        ${LimboIddqdWorkspaceViewBaseElement.styles}

        table td {
            vertical-align: top;
        }

    `;

}

customElements.define("limbo-iddqd-user-workspace-view", LimboIddqdUserWorkspaceViewElement);

export default LimboIddqdUserWorkspaceViewElement;
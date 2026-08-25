import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";
import { extractUmbColorVariable } from "@umbraco-cms/backoffice/resources";




import { UMB_WORKSPACE_CONTEXT } from '@umbraco-cms/backoffice/workspace';
//import { UMB_APP_HEADER_CONTEXT } from '@umbraco-cms/backoffice/app-header';

import { IddqdService } from "@limbo/iddqd/service";

function parseIcon(value) {
    if (!value) return null;
    const array = value.split(" ");
    const color = array.length > 1 ? extractUmbColorVariable(array[1].replace("color-", "")) : null;
    return { name: array[0], color: color ? `color:var(${color})` : null };
}

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function waitAtLeast(promise, minTime = 500) {
    const [result] = await Promise.all([
        promise,
        delay(minTime)
    ]);
    return result;
}

export default class IddqdDomainsWorkspaceView extends UmbElementMixin(LitElement) {

    constructor() {
        super();
        this.domains = [];
        this.groups = [];
    }

    async connectedCallback() {
        await super.connectedCallback();
        document.title = "Domains | Iddqd | Umbraco"; // not sure if this is the correct place to set the title, but it works for now ¯\_(ツ)_/¯
        await this.updateList();
    }

    provideContext() {
        // You can leave this empty for now
    }

    async updateList(delay) {
        const self = this;
        if (self.loading) return;
        self.loading = true;
        self.requestUpdate();
        self.domains = await waitAtLeast(IddqdService.domains.getAll(), delay ?? 10);
        self.groups = [];
        self.domains.forEach(function (domain) {
            if (domain.rootContent) domain.rootContent.icon = parseIcon(domain.rootContent.contentType?.icon);
            const groupName = domain.rootContentId?.toString() ?? null;
            let group = self.groups.find(g => g.rootContentId === domain.rootContentId);
            if (!group) {
                group = {
                    rootContentId: domain.rootContentId,
                    rootContent: domain.rootContent,
                    languageIsoCode: domain.languageIsoCode,
                    languageName: domain.languageName,
                    domains: []
                };
                self.groups.push(group);
            }
            group.domains.push(domain);
        });
        self.loading = false;
        self.refreshButtonState = null;
        self.requestUpdate();
    }

    onRefreshClicked() {
        this.refreshButtonState = "waiting";
        this.updateList(250);
    }

    onGroupByRootContentChanged(e) {
        this.groupByRootContent = e.target.checked;
        this.requestUpdate();
    }

    render() {

        return html`
            <div class="stack">
                <header>
                    <h2>Domains</h2>
                    <div class="actions">
                        <uui-checkbox @change=${e => this.onGroupByRootContentChanged(e)} label="Group by root content?"></uui-checkbox>
                        <uui-button look="outline" state="${this.refreshButtonState}" @click="${(e) => this.onRefreshClicked(e)}" label="Refresh"></uui-button>
                    </div>
                </header>
                ${when(this.groupByRootContent, () => html`
                    <div class="stack">
                        ${repeat(this.groups, (group) => html`
                            <div style="display: flex; gap: 10px;">
                                ${when(group.rootContent, () => html`
                                    <h3 style="margin: 0;">
                                        <a href="${group.rootContent.editUrl}">${group.rootContent.name}</a>
                                    </h3>
                                    ${when(group.rootContent?.contentType, (ct) => html`
                                        <div>
                                            &mdash;
                                            ${group.rootContent?.icon ? html`<uui-icon name="${group.rootContent.icon.name}" style="${group.rootContent.icon.color}"></uui-icon>` : ''}
                                            ${ct.name}
                                            <span class="muted">(<span class="select-all">${ct.alias}</span>)</span>
                                        </div>
                                    `)}
                                `)}
                            </div>
                            <uui-box>
                                <uui-table>
                                    <uui-table-head>
                                        <uui-table-head-cell>ID</uui-table-head-cell>
                                        <uui-table-head-cell>Domain</uui-table-head-cell>
                                        <uui-table-head-cell>Language</uui-table-head-cell>
                                    </uui-table-head>
                                    ${repeat(group.domains, domain => html`
                                        <uui-table-row>
                                            <uui-table-cell>${domain.id}</uui-table-cell>
                                            <uui-table-cell class="fw">
                                                ${when(domain.name === domain.nameAscii, () => html`
                                                    <span class="select-all">${domain.name}</span>
                                                `, () => html`
                                                    <span class="select-all">${domain.name}</span>
                                                    <small class="muted">(<span class="select-all">${domain.nameAscii}</span>)</small>
                                                `)}
                                            </uui-table-cell>
                                            <uui-table-cell class="nw">
                                                ${when(domain.languageName, () => html`
                                                    <span>${domain.languageName}</span>
                                                    <small class="muted">(<span class="select-all">${domain.languageIsoCode}</span>)</small>
                                                `, () => html`
                                                    <span class="error">${domain.languageIsoCode}</span>
                                                `)}
                                            </uui-table-cell>
                                        </uui-table-row>
                                    `)}
                                </uui-table>
                            </uui-box>
                        `)}
                    </div>
                `, () => html`
                    <uui-box>
                        <uui-table>
                            <uui-table-head>
                                <uui-table-head-cell>ID</uui-table-head-cell>
                                <uui-table-head-cell>Domain</uui-table-head-cell>
                                <uui-table-head-cell>Root Content</uui-table-head-cell>
                                <uui-table-head-cell>Language</uui-table-head-cell>
                            </uui-table-head>
                            ${repeat(this.domains, domain => html`
                                <uui-table-row>
                                    <uui-table-cell>${domain.id}</uui-table-cell>
                                    <uui-table-cell class="nw">
                                        ${when(domain.name === domain.nameAscii, () => html`
                                            <span class="select-all">${domain.name}</span>
                                        `, () => html`
                                            <span class="select-all">${domain.name}</span>
                                            <small class="muted">(<span class="select-all">${domain.nameAscii}</span>)</small>
                                        `)}
                                    </uui-table-cell>
                                    <uui-table-cell class="fw">
                                        ${when(domain.rootContent, () => html`
                                            <a href="${domain.rootContent.editUrl}"><strong>${domain.rootContent.name}</strong></a>
                                            ${when(domain.rootContent.contentType, (ct) => html`
                                                &mdash;
                                                ${domain.rootContent?.icon ? html`<uui-icon name="${domain.rootContent.icon.name}" style="${domain.rootContent.icon.color}"></uui-icon>` : ''}
                                                ${ct.name}
                                                <span class="muted">(<span class="select-all">${ct.alias}</span>)</span>
                                            `)}
                                        `, () => html`
                                            <span class="error">Root content not found.</span>
                                        `)}
                                    </uui-table-cell>
                                    <uui-table-cell class="nw">
                                        ${when(domain.languageName, () => html`
                                            <span>${domain.languageName}</span>
                                            <small class="muted">(<span class="select-all">${domain.languageIsoCode}</span>)</small>
                                        `, () => html`
                                            <span class="error">${domain.languageIsoCode}</span>
                                        `)}
                                    </uui-table-cell>
                                </uui-table-row>
                            `)}
                        </uui-table>
                    </uui-box>
                `)}
            </div>
        `;

    }

    static styles = css`

        :host > div {
            padding: 20px;
        }

        header {
            display: flex;
            h2 {
                flex: 1;
            }
        }

        h3 {
            margin-bottom: 0;
        }

        a {
            color: black;
            text-decoration: none;
            &:hover {
                text-decoration: underline;
            }
        }

        uui-box {
            --uui-box-default-padding: 0;
        }

        uui-table-head-cell {
            --uui-table-cell-padding: 5px 15px;
        }

        .stack {
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        .actions {
            display: flex;
            gap: 10px;
            align-items: center;
            justify-content: end;
        }

        .error {
            color: red;
        }

        .muted {
            color: #999;
        }

        .select-all {
            user-select: all;
        }

        .fw {
            width: 100%;
        }
        .nw {
            white-space: nowrap;
        }

    `;

}

customElements.define("iddqd-domains-workspace-view", IddqdDomainsWorkspaceView);
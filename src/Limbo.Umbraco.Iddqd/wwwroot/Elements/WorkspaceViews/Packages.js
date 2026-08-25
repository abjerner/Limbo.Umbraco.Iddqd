import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbPackageRepository } from '@umbraco-cms/backoffice/package';

import { IddqdService } from "@limbo/iddqd/service";

function dictionary(array, keySelector) {
    array.forEach(function (item) {
        const key = keySelector(item);
        array[key] = item;
    });
    return array;
}

function orderBy(array, keySelector, descending = false) {
    return [...array].sort((a, b) => {
        const keyA = keySelector(a);
        const keyB = keySelector(b);
        if (keyA < keyB) return descending ? 1 : -1;
        if (keyA > keyB) return descending ? -1 : 1;
        return 0;
    });
}

function groupBy(array, keySelector) {
    const map = new Map();
    for (const item of array) {
        const key = keySelector(item);
        if (!map.has(key)) map.set(key, []);
        map.get(key).push(item);
    }
    return Array.from(map, ([key, value]) => ({ key, value }));
}

export default class IddqdPackagesWorkspaceView extends UmbElementMixin(LitElement) {

    constructor() {

        super();

        const self = this;

        this.packages = [];
        this.groups = [];

        this.groupBy = "none";
        this.sortBy = "alias";
        this.orderBy = "asc";

        this.groupOptions = dictionary([
            { alias: "none", name: "None" },
            { alias: "company", name: "Company" },
            { alias: "product", name: "Product" }
        ], x => x.alias);

        this.sortOptions = dictionary([
            { alias: "alias", name: "Alias" },
            { alias: "name", name: "Name" },
            { alias: "assembly", name: "Assembly" }
        ], x => x.alias);

        this.orderOptions = dictionary([
            { alias: "asc", name: "Ascending" },
            { alias: "desc", name: "Descending" }
        ], x => x.alias);

        this.packageRepository = new UmbPackageRepository(this);

        this.packageRepository.rootItems().then(function (hej) {
            self.observe(hej, function (data) {
                IddqdService.packages.getManifests().then(function (res) {
                    const manifests = {};
                    res.forEach(function (item) {
                        if (item.id) manifests[item.id] = item;
                        manifests[item.name] = item;
                    });
                    data.forEach(function (item) {
                        let hej = null;
                        if (item.id) hej = manifests[item.id];
                        if (!hej && item.name) hej = manifests[item.name];
                        item.assembly = hej?.assembly;
                        item.description = hej?.assembly?.description;
                        item.company = hej?.assembly?.company;
                        item.product = hej?.assembly?.product;
                        item.authors = hej?.assembly?.authors;
                        item.packageProjectUrl = hej?.assembly?.packageProjectUrl;
                        item.repositoryUrl = hej?.assembly?.repositoryUrl;
                        item.marketplaceUrl = hej?.assembly?.marketplaceUrl;
                        item.documentationUrl = hej?.assembly?.documentationUrl;
                        item.nuGetUrl = hej?.assembly?.nuGetUrl;
                        item.allowTelemetry = hej?.allowTelemetry;
                        item.links = [
                            { name: "Website", icon: "icon-globe", url: item.packageProjectUrl },
                            { name: "Documentation", icon: "icon-book", url: item.documentationUrl },
                            { name: "Repository", icon: item.repositoryUrl?.indexOf("github.com") !== -1 ? "icon-github" : "icon-forking", url: item.repositoryUrl },
                            { name: "Umbraco Marketplace", icon: "icon-store", url: item.marketplaceUrl },
                            { name: "NuGet", icon: "icon-iddqd-nuget", url: item.nuGetUrl }
                        ];
                    });
                    self.packages = data;
                    self.updateList();
                });
            });
        });

    }

    async connectedCallback() {
        await super.connectedCallback();
        document.title = "Packages | Iddqd | Umbraco"; // not sure if this is the correct place to set the title, but it works for now ¯\_(ツ)_/¯
        this.updateList();
    }

    provideContext() {
        // You can leave this empty for now
    }

    updateList() {

        // Filter the items based on the search text
        let items = this.text ? this.packages.filter(item =>
            Object.values(item).some(value =>
                String(value).toLowerCase().includes(this.text.toLowerCase())
            )
        ) : this.packages;

        // Sort the items based on the selected sort option
        if (this.sortBy == "name") {
            items = orderBy(items, item => item.name, this.sortOrder === "desc");
        } else if (this.sortBy == "assembly") {
            items = orderBy(items, item => item.assembly, this.sortOrder === "desc");
        } else {
            items = orderBy(items, item => item.alias ?? item.id ?? item.name, this.sortOrder === "desc");
        }

        // Group the items based on the selected group option
        this.groups = [];
        if (this.groupBy === "company") {
            this.groups = groupBy(items, item => item.company);
        } else if (this.groupBy === "product") {
            this.groups = groupBy(items, item => item.product);
        } else {
            this.groups = [{ key: "", value: items }];
        }

        // Sort the groups in ascending order, but put the "Not specified" group (with an empty key) at the end
        this.groups = orderBy(this.groups, x => x.key ? "0" + x.key : "1");

        // Request a new UI update
        this.requestUpdate();

    }

    onKeyUp(e) {
        this.text = e.target.value;
        this.updateList();
    }

    setGroupBy(groupBy, e) {
        this.groupBy = groupBy;
        this.updateList();
        const popover = e.target.closest("uui-popover-container");
        if (popover) popover.hidePopover();
    }

    setSortBy(sortBy, e) {
        this.sortBy = sortBy;
        this.updateList();
        const popover = e.target.closest("uui-popover-container");
        if (popover) popover.hidePopover();
    }

    setOrderBy(orderBy, e) {
        this.orderBy = orderBy;
        this.updateList();
        const popover = e.target.closest("uui-popover-container");
        if (popover) popover.hidePopover();
    }

    render() {

        const typeToSearch = this.localize.term("general_typeToSearch");

        return html`
            <div class="stack">
                <header>
                    <h2>Packages</h2>
                </header>
                <div class="filters">
                    <uui-input label="${typeToSearch}" style="flex: 1;" placeholder="${typeToSearch}" @keyup=${(e) => this.onKeyUp(e)}></uui-input>
                    <uui-button look="outline" popovertarget="groupByPopover" label="Group by">
                        Group by:
                        <strong>${this.groupOptions[this.groupBy]?.name ?? this.sortBy}</strong>
                    </uui-button>
                    <uui-button look="outline" popovertarget="sortByPopover" label="Sort by">
                        Sort by:
                        <strong>${this.sortOptions[this.sortBy]?.name ?? this.sortBy}</strong>
                    </uui-button>
                    <uui-button look="outline" popovertarget="sortOrderPopover" label="Order by">
                        Order by:
                        <strong>${this.orderOptions[this.orderBy]?.name ?? this.orderBy}</strong>
                    </uui-button>
                </div>
                <uui-popover-container id="groupByPopover" placement="bottom-end">
                    <uui-box style="margin-top: 5px;">
                        <div style="display: flex; flex-direction: column;">
                            ${repeat(this.groupOptions, (option) => html`
                                <uui-button look="secondary" @click=${(e) => this.setGroupBy(option.alias, e)} label="${option.name}"></uui-button>
                            `)}
                        </div>
                    </uui-box>
                </uui-popover-container>
                <uui-popover-container id="sortByPopover" placement="bottom-end">
                    <uui-box style="margin-top: 5px;">
                        <div style="display: flex; flex-direction: column;">
                            ${repeat(this.sortOptions, (option) => html`
                                <uui-button look="secondary" @click=${(e) => this.setSortBy(option.alias, e)} label="${option.name}"></uui-button>
                            `)}
                        </div>
                    </uui-box>
                </uui-popover-container>
                <uui-popover-container id="sortOrderPopover" placement="bottom-end">
                    <uui-box style="margin-top: 5px;">
                        <div style="display: flex; flex-direction: column; background: #fff;">
                            ${repeat(this.orderOptions, (option) => html`
                                <uui-button look="secondary" @click=${(e) => this.setOrderBy(option.alias, e)} label="${option.name}"></uui-button>
                            `)}
                        </div>
                    </uui-box>
                </uui-popover-container>
                ${when(this.groups?.length > 0, () => html`
                    <div class="stack results">
                        ${repeat(this.groups, (group) => html`
                            ${when(group.key || this.groupBy !== "none", () => html`
                                <h3>${group.key || 'Not specified'}</h3>
                            `)}
                            <uui-box>
                                <table class="table list">
                                    <thead>
                                        <tr>
                                            <th>Package</th>
                                            <th>Version</th>
                                            ${when(this.groupBy !== "company", () => html`<th>Company</th>`)}
                                            <th>Authors</th>
                                            <th>Telemetry</th>
                                            <th>Links</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        ${repeat(group.value, (p) => html`
                                            <tr>
                                                <td class="nw">
                                                    <strong>${p.name}</strong><br />
                                                    ${when(p.assembly, () => html`
                                                        <small>${p.assembly.name}.dll</small>
                                                    `)}
                                                </td>
                                                <td class="nw">${p.version}</td>
                                                ${when(this.groupBy !== "company", () => html`
                                                    <td class="nw">
                                                        ${when(p.assembly?.company, () => html`
                                                            ${p.assembly?.company}
                                                        `, () => html`
                                                            <span class="muted">N/A<span>
                                                        `)}
                                                    </td>
                                                `)}
                                                <td class="nw">
                                                        ${when(p.assembly?.authors, () => html`
                                                            ${p.assembly?.authors}
                                                        `, () => html`
                                                            <span class="muted">N/A<span>
                                                        `)}
                                                    </td>
                                                <td class="fw">${p.allowTelemetry ? "Enabled" : "Disabled"}</td>
                                                <td class="nw tar">
                                                    ${repeat(p.links, (link) => html`
                                                        <uui-button href="${link.url}" .disabled=${!link.url} look="secondary" target="_blank" rel="noreferrer noopener" label="${link.name}" title="${link.name}">
                                                            <uui-icon name="${link.icon}"></uui-icon>
                                                        </uui-button>
                                                    `)}
                                                </td>
                                            </tr>
                                        `)}
                                    </tbody>
                                </table>
                            </uui-box>
                        `)}
                    </div>
                `)}
            </div>
        `;

    }

    static styles = css`

        :host > div {
            padding: 20px;
        }

        uui-box {
            --uui-box-default-padding: 0;
        }

        .stack {
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        .results {
            h3 {
                margin: 0;
            }
        }

        .filters {
            display: flex;
            gap: 10px;
            > uui-button {
                font-weight: normal;
            }
        }

        .muted {
            color: #999;
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

        .nw {
            white-space: nowrap;
        }

        .fw  {
            width: 100%;
        }

        .tar {
            text-align: right;
        }

    `;

}

customElements.define("iddqd-packages-workspace-view", IddqdPackagesWorkspaceView);
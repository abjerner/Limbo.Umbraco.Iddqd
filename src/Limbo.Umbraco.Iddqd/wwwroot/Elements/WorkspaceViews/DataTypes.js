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

function filterItems(items, text) {
    if (!text) return items;
    return items.filter(x => Object
        .values(x)
        .some(value => String(value).toLowerCase().includes(text.toLowerCase())));
}

export default class IddqdDataTypesWorkspaceView extends UmbElementMixin(LitElement) {

    constructor() {

        super();

        const self = this;

        this.dataTypes = [];
        this.groups = [];

        this.groupBy = "none";
        this.sortBy = "name";
        this.orderBy = "asc";

        this.groupOptions = dictionary([
            { alias: "none", name: "None" },
            { alias: "editorAlias", name: "Editor alias" },
            { alias: "editorUiAlias", name: "Editor UI alias" },
            { alias: "company", name: "Company" },
            { alias: "product", name: "Product" }
        ], x => x.alias);

        this.sortOptions = dictionary([
            { alias: "name", name: "Name" },
            { alias: "editorAlias", name: "Editor alias" },
            { alias: "editorUiAlias", name: "Editor UI alias" },
            { alias: "name", name: "Name" },
            { alias: "assembly", name: "Assembly" }
        ], x => x.alias);

        this.orderOptions = dictionary([
            { alias: "asc", name: "Ascending" },
            { alias: "desc", name: "Descending" }
        ], x => x.alias);

    }

    async connectedCallback() {
        await super.connectedCallback();
        document.title = "Data Types | Iddqd | Umbraco"; // not sure if this is the correct place to set the title, but it works for now ¯\_(ツ)_/¯
        this.updateList();
    }

    provideContext() {
        // You can leave this empty for now
    }

    async updateList() {

        this.dataTypes = await IddqdService.dataTypes.getAll();

        // Filter the items based on the search text
        let items = filterItems(this.dataTypes, this.text);

        // Sort the items based on the selected sort option
        if (this.sortBy == "name") {
            items = orderBy(items, item => item.name, this.sortOrder === "desc");
        } else if (this.sortBy == "assembly") {
            items = orderBy(items, item => item.editor?.assembly, this.sortOrder === "desc");
        } else {
            items = orderBy(items, item => item.editorAlias ?? item.id ?? item.name, this.sortOrder === "desc");
        }

        // Group the items based on the selected group option
        this.groups = [];
        if (this.groupBy === "editorAlias") {
            this.groups = groupBy(items, item => item.editorAlias);
        } else if (this.groupBy === "editorUiAlias") {
            this.groups = groupBy(items, item => item.editorUiAlias);
        } else if (this.groupBy === "company") {
            this.groups = groupBy(items, item => item.editor?.assembly.company);
        } else if (this.groupBy === "product") {
            this.groups = groupBy(items, item => item.editor?.assembly.product);
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
                    <h2>Data Types</h2>
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
                                            <th>ID / Key</th>
                                            <th>Name</th>
                                            <th>Value Type</th>
                                            ${when(this.groupBy !== "company", () => html`<th>Company</th>`)}
                                            ${when(this.groupBy !== "product", () => html`<th>Product</th>`)}
                                        </tr>
                                    </thead>
                                    <tbody>
                                        ${repeat(group.value, (dt) => html`
                                            <tr>
                                                <td>
                                                    ${dt.id}<br />
                                                    <small class="details muted select-all">${dt.key}</small>
                                                </td>
                                                <td>
                                                    <strong>${dt.name}</strong><br />
                                                    <small class="details muted select-all">${dt.editorAlias} | ${dt.editorUiAlias}</small>
                                                </td>
                                                <td>
                                                    ${when(dt.editor && dt.editor.databaseType != dt.databaseType, () => html`
                                                        <code class="danger">${dt.databaseType}</code>
                                                    `, () => html`
                                                        <code class="success">${dt.databaseType}</code>
                                                    `)}
                                                </td>
                                                ${when(this.groupBy !== "company", () => html`
                                                    <td>${this.renderValue(dt.editor?.assembly?.company)}</td>
                                                `)}
                                                ${when(this.groupBy !== "product", () => html`
                                                    <td>${this.renderValue(dt.editor?.assembly?.product)}</td>
                                                `)}
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

    renderValue(value) {
        if (!value) return html`<em class="muted">N/A</em>`;
        return html`${value}`;
    }

    static styles = css`

        :host {
            --success: #16a34a;
            --warning: #f59e0b;
            --danger: #dc2626;
            --info: #0ea5e9;
            --neutral: #64748b;
        }

        .select-all {
            user-select: all;
        }

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

        code {
            border-radius: 4px;
            padding: 2px 5px;
            background: rgba(127,127,127, .10);
            border: 1px solid rgba(127,127,127, .35);
            white-space: nowrap;
        }

        code.danger {
            // TODO: match colors to Umbraco (UUI)
            --danger: #dc2626;
            --lb-bg: rgba(220,38,38,.16);
            --lb-border: rgba(220,38,38,.42);
            --lb-fg: var(--danger);


            --lb-bg: var(--uui-color-danger);
            --lb-fg: #fff;
            --lb-border: var(--uui-color-danger);
            border: 1px solid var(--lb-border);
            background: var(--lb-bg);
            color: var(--lb-fg);
        }

        code.success {
            // TODO: match colors to Umbraco (UUI)
            --lb-bg: rgba(34, 197, 94, .16);
            --lb-border: rgba(34, 197, 94, .40);
            --lb-fg: var(--success);
            --lb-bg: var(--uui-color-positive);
            --lb-fg: #fff;
            --lb-border: var(--uui-color-positive);
            border: 1px solid var(--lb-border);
            background: var(--lb-bg);
            color: var(--lb-fg);
        }

    `;

}

customElements.define("iddqd-data-types-workspace-view", IddqdDataTypesWorkspaceView);
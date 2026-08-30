import { html, css, repeat, when, nothing } from "@umbraco-cms/backoffice/external/lit";
import { IddqdService } from "@limbo/iddqd/service";
import { LimboIddqdWorkspaceViewBaseElement } from "./Base.js";

export class LimboIddqdDataTypeWorkspaceViewElement extends LimboIddqdWorkspaceViewBaseElement {

    constructor() {
        super();
    }

    async init(type, key) {
        if (type != "data-type") return;
        console.log("Initializing for data type with key", key);
        this.dataType = await IddqdService.dataTypes.get(key);
        this.relations = await IddqdService.dataTypes.getRelations(key);
        this.requestUpdate();
    }

    render() {
        return html`
            <div>
                ${when(this.dataType, () => html`
                    ${this.renderDataType(this.dataType)}
                    ${this.renderDataEditor(this.dataType.editor, this.dataType)}
                    ${this.renderAssembly(this.dataType.editor?.assembly)}
                    ${this.renderRelations()}
                `)}
            </div>
        `;
    }

    renderValue(value) {
        if (!value) return html`<em class="muted">N/A</em>`;
        return html`${value}`;
    }

    renderDataType(dataType) {

        return html`
            <uui-box headline="Data Type">
                <uui-box-body>
                    <table class="details">
                        <tr>
                            <th>ID</th>
                            <td>
                                <code class="select-all">${this.dataType.id}</code>
                            </td>
                        </tr>
                        <tr>
                            <th>Key</th>
                            <td>
                                <code class="select-all">${this.dataType.key}</code>
                            </td>
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
                            <th>Database Type</th>
                            <td>
                                ${when(this.dataType.editor && this.dataType.editor.databaseType != this.dataType.databaseType, () => html`
                                    <code class="danger">${this.dataType.databaseType}</code>
                                    <span>incorrect according to underlying data editor. Save the data type again to fix...</span>
                                `, () => html`
                                    <code class="success">${this.dataType.databaseType}</code>
                                    <span>matches data editor</span>
                                `)}
                            </td>
                        </tr>
                        ${when(this.dataType.createDate, () => html`
                            <tr>
                                <th>Created</th>
                                <td>
                                    ${this.date(this.dataType.createDate)}
                                    <small>(${this.fromNow(this.dataType.createDate)})</small>
                                </td>
                            </tr>
                        `)}
                    </table>
                </uui-box-body>
            </uui-box>
        `;

    }

    renderDataEditor(editor, dataType) {

        if (!editor) {
            return html`
                <div class="alert alert-danger">
                    The alias <strong>${dataType.editorAlias}</strong> does not match a known data editor.
                </div>
            `;
        }

        return html`
            <uui-box headline="Data Editor">
                <uui-box-body>
                    <table class="details">
                        <tr>
                            <th>Alias</th>
                            <td>
                                <code class="select-all">${editor.alias}</code>
                            </td>
                        </tr>
                        <tr>
                            <th>Type</th>
                            <td>
                                <code class="select-all">${editor.type}</code>
                            </td>
                        </tr>
                        <tr>
                            <th>Readonly?</th>
                            <td>${editor.isReadOnly}</td>
                        </tr>
                        <tr>
                            <th>Value Type</th>
                            <td><code class="select-all">${editor.valueType}</code></td>
                        </tr>
                        <tr>
                            <th>Database Type</th>
                            <td><code class="select-all">${editor.databaseType}</code></td>
                        </tr>
                    </table>
                </uui-box-body>
            </uui-box>
        `;

    }

    renderAssembly(assembly) {

        if (!assembly) return nothing;

        return html`
            <uui-box headline="Assembly">

                <div slot="header">
                    ${when(assembly.links?.length, () => html`
                        <div class="fassembly-links">
                            ${repeat(assembly.links, (link) => html`
                                <uui-button href="${link.url}" .disabled=${!link.url} look="outline"  target="_blank" rel="noreferrer noopener" label="${link.name}" title="${link.name}">
                                    <uui-icon name="${link.icon}"></uui-icon>
                                </uui-button>
                            `)}
                        </div>
                    `)}
                </div>
                <uui-box-body>
                    <table class="details">
                        <tr>
                            <th>Name</th>
                            <td>${assembly.name}</td>
                        </tr>
                        <tr>
                            <th>Title</th>
                            <td>${this.renderValue(assembly.title)}</td>
                        </tr>
                        <tr>
                            <th>Description</th>
                            <td>${this.renderValue(assembly.description)}</td>
                        </tr>
                        <tr>
                            <th>Version</th>
                            <td>${assembly.version}</td>
                        </tr>
                        <tr>
                            <th>Configuration</th>
                            <td>
                                ${when(assembly.configuration === "Release", () => html`
                                    <code class="success">${assembly.configuration}</code>
                                `, () => html`
                                    <code class="danger">${assembly.configuration}</code>
                                    <span>should be <strong>Release</strong>...</span>
                                `)}
                            </td>
                        </tr>
                        <tr>
                            <th>Company</th>
                            <td>${this.renderValue(assembly.company)}</td>
                        </tr>
                        <tr>
                            <th>Product</th>
                            <td>${this.renderValue(assembly.product)}</td>
                        </tr>
                        <tr>
                            <th>Authors</th>
                            <td>${this.renderValue(assembly.authors)}</td>
                        </tr>
                    </table>
                </uui-box-body>
            </uui-box>
        `;

    }

    renderRelations() {

        if (!this.relations) return html``;

        return html`
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

    static styles = css`

        ${LimboIddqdWorkspaceViewBaseElement.styles}

        :host {
            --success: #16a34a;
            --warning: #f59e0b;
            --danger: #dc2626;
            --info: #0ea5e9;
            --neutral: #64748b;
        }

        div[slot='header'] {
            flex: 1;
            justify-items: end;
            margin: -4px 0;
        }

        div.alert.alert-danger {
            margin-top: 20px;
            background: var(--uui-color-danger);
            color: var(--uui-color-danger-contrast);
            padding: 20px;
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

customElements.define("limbo-iddqd-data-type-workspace-view", LimboIddqdDataTypeWorkspaceViewElement);

export default LimboIddqdDataTypeWorkspaceViewElement;
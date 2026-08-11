import { html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";
import { IddqdService } from "@limbo/iddqd/service";
import { LimboIddqdWorkspaceViewBaseElement } from "./Base.js";

export class LimboIddqdDataTypeWorkspaceViewElement extends LimboIddqdWorkspaceViewBaseElement {

    constructor() {
        super();
    }

    async init(type, key) {
        if (type != "data-type") return;
        console.log("Initializing for data type with key", key);
        this.dataType = await IddqdService.getDataType(key);
        //this.relations = await IddqdService.getDataTypeRelations(key);
        this.requestUpdate();
    }

    render() {
        return html`
            <div>
                ${when(this.dataType, () => html`
                     ${this.renderDetails()}
                     ${this.renderRelations()}
                `)}
            </div>
        `;
    }

    renderDetails() {
        return html`
            <uui-box headline="Data Type">
                <uui-box-body>
                    <table class="details">
                        <tr>
                            <th>ID</th>
                            <td>
                                <span class="select-all">${this.dataType.id}</span>
                            </td>
                        </tr>
                        <tr>
                            <th>Key</th>
                            <td>
                                <span class="select-all">${this.dataType.key}</span>
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

}

customElements.define("limbo-iddqd-data-type-workspace-view", LimboIddqdDataTypeWorkspaceViewElement);

export default LimboIddqdDataTypeWorkspaceViewElement;
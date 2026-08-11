import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";

//import { getDisplayStateFromUserStatus } from "@umbraco-cms/backoffice/user";

import { UMB_ENTITY_WORKSPACE_CONTEXT, UMB_ROUTABLE_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/workspace";

import { IddqdService } from "@limbo/iddqd/service";

import { LimboIddqdWorkspaceViewBaseElement } from "./Base.js";

function inspectUmbracoVariables(
    element = document.querySelector("umb-app") || document.documentElement,
    prefixes = ["--uui", "--umb"]
) {
    const variables = new Map();

    function visitRules(rules, stylesheet) {
        for (const rule of rules) {
            // Normal style rules
            if (rule instanceof CSSStyleRule) {
                for (const property of rule.style) {
                    if (!prefixes.some(prefix => property.startsWith(prefix))) {
                        continue;
                    }

                    if (!variables.has(property)) {
                        variables.set(property, {
                            umbraco: [],
                            custom: []
                        });
                    }

                    const declaration = {
                        scope: rule.selectorText,
                        value: rule.style.getPropertyValue(property).trim(),
                        stylesheet
                    };

                    const bucket = stylesheet.toLowerCase().includes("/umbraco/")
                        ? variables.get(property).umbraco
                        : variables.get(property).custom;

                    bucket.push(declaration);
                }
            }

            // Recurse into @media, @layer, @supports, etc.
            else if ("cssRules" in rule && rule.cssRules) {
                visitRules(rule.cssRules, stylesheet);
            }
        }
    }

    for (const sheet of document.styleSheets) {
        let rules;

        try {
            rules = sheet.cssRules;
        } catch {
            // Cross-origin stylesheet
            continue;
        }

        visitRules(rules, sheet.href || "<inline>");
    }

    const styles = getComputedStyle(element);

    const results = Array.from(variables.entries())
        .map(([variable, info]) => ({
            variable,
            computed: styles.getPropertyValue(variable).trim(),
            umbraco: info.umbraco,
            custom: info.custom
        }))
        .sort((a, b) => a.variable.localeCompare(b.variable));

    // console.table(
    //     results.map(r => ({
    //         Variable: r.variable,
    //         Computed: r.computed,
    //         Umbraco: r.umbraco.length,
    //         Custom: r.custom.length
    //     }))
    // );

    // console.log(results);

    return results;
}















export class LimboIddqdMemberTypeWorkspaceViewElement extends LimboIddqdWorkspaceViewBaseElement {

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

    async init(type, key) {

        if (type !== "member-type") return;

        this.memberType = await IddqdService.memberTypes.get(key);

        this.memberType.createDate = new Date(this.memberType.createDate);
        this.memberType.updateDate = new Date(this.memberType.updateDate);

        this.requestUpdate();

    }

    render() {
        return html`
            <div>
                ${when(this.memberType, () => html`
                     ${this.renderDetails()}
                     ${this.renderVariables()}
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
                            <td><span class="select-all">${this.memberType.id}</span></td>
                        </tr>
                        <tr>
                            <th>Key</th>
                            <td><span class="select-all">${this.memberType.key}</span></td>
                        </tr>
                        <tr>
                            <th>Alias</th>
                            <td><span class="select-all">${this.memberType.alias}</span></td>
                        </tr>
                        <tr>
                            <th>Icon</th>
                            <td><span class="select-all">${this.memberType.icon}</span></td>
                        </tr>
                        <tr>
                            <th>Name</th>
                            <td>${this.memberType.name}</td>
                        </tr>
                        <tr>
                            <th>Created</th>
                            <td>
                                ${this.date(this.memberType.createDate)}
                                <small>(${this.fromNow(this.memberType.createDate)})</small>
                            </td>
                        </tr>
                        <tr>
                            <th>Updated</th>
                            <td>
                                ${this.date(this.memberType.updateDate)}
                                <small>(${this.fromNow(this.memberType.updateDate)})</small>
                            </td>
                        </tr>
                    </table>
                </uui-box-body>
            </uui-box>
        `;

    }

    renderVariables() {

        return html``;

        const result = inspectUmbracoVariables();

        return html`

            <table class="table list">
                ${repeat(result/*.filter(x => x.custom?.length > 0)*/, (variable) => html`
                    <tr>
                        <td>
                            <pre>${JSON.stringify(variable, null, 2)}</pre>
                        </td>
                    </tr>
                `)}
            </table>

        `;

    }

}

customElements.define("limbo-iddqd-member-type-workspace-view", LimboIddqdMemberTypeWorkspaceViewElement);

export default LimboIddqdMemberTypeWorkspaceViewElement;
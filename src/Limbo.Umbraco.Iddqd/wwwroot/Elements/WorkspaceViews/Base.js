import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";

//import { getDisplayStateFromUserStatus } from "@umbraco-cms/backoffice/user";

import { UMB_ENTITY_WORKSPACE_CONTEXT, UMB_ROUTABLE_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/workspace";

import { IddqdService } from "@limbo/iddqd/service";

const dateOptions = {
    day: "numeric",
    hour: "numeric",
    minute: "numeric",
    month: "long",
    second: "numeric",
    year: "numeric"
};

export class LimboIddqdWorkspaceViewBaseElement extends UmbElementMixin(LitElement) {

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

    async init(type, key) { }

    date(value) {
        if (!value) return "";
        const date = new Date(value);
        return this.localize.date(date, dateOptions);
    }

    fromNow(value) {
        if (!value) return "";
        const date = new Date(value);
        const seconds = Math.floor((Date.now() - date.getTime()) / 1000);
        const rtf = new Intl.RelativeTimeFormat(this.localize.lang(), { numeric: "auto" });
        const abs = Math.abs(seconds);
        if (abs < 60) return rtf.format(-seconds, "second");
        if (abs < 3600) return rtf.format(-Math.floor(seconds / 60), "minute");
        if (abs < 86400) return rtf.format(-Math.floor(seconds / 3600), "hour");
        return rtf.format(-Math.floor(seconds / 86400), "day");
    }

    renderValue(value, type) {

        if (!value) return html`<em class="muted">N/A</em>`;

        if (value instanceof Date) return html`${this.date(value)} <small>(${this.fromNow(value)})</small>`;

        if (type === "id" || type === "key" || type === "username" || type === "email") return html`<span class="select-all">${value}</span>`;

        return html`${value}`;

    }

    renderDetail(name, value, type) {
        return html`
            <tr>
                <th>${name}</th>
                <td>${this.renderValue(value, type)}</td>
            </tr>
        `;
    }

    static styles = css`

        :host > div {
            padding: 20px;
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

        .select-all {
            user-select: all;
        }

    `;

}

export default LimboIddqdWorkspaceViewBaseElement;
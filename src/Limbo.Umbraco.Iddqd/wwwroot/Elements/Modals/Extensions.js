import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

import {
    css,
    html
} from "@umbraco-cms/backoffice/external/lit";

import {
    UmbLitElement
} from "@umbraco-cms/backoffice/lit-element";

export const EXTENSION_DETAILS_MODAL = new UmbModalToken(
    "Limbo.Umbraco.Iddqd.Extensions.Modal", {
        modal: {
            type: "sidebar",
            size: "medium"
        }
    }
);

class ExtensionDetailsModalElement extends UmbLitElement {

    static properties = {
        modalContext: { attribute: false }
    };

    get extension() {
        return this.modalContext?.data?.extension;
    }

    render() {
        const extension = this.extension;

        if (!extension) {
            return html`
                <umb-body-layout headline="Extension details">
                    <uui-box>
                        No extension information was supplied.
                    </uui-box>
                </umb-body-layout>
            `;
        }

        return html`
            <umb-body-layout
                headline=${extension.name ?? extension.alias ?? "Extension details"}>

                <div class="content">

                    <uui-box headline="Extension">
                        <dl>
                            ${this.renderProperty("Type", extension.type)}
                            ${this.renderProperty("Name", extension.name)}
                            ${this.renderProperty("Alias", extension.alias)}
                            ${this.renderProperty("Weight", extension.weight)}
                            ${this.renderProperty("Kind", extension.kind)}
                        </dl>
                    </uui-box>

                    <uui-box headline="Additional information">
                        <dl>
                            ${this.renderProperty(
            "Has element",
            extension.element ? "Yes" : "No"
        )}

                            ${this.renderProperty(
            "Has API",
            extension.api ? "Yes" : "No"
        )}

                            ${this.renderProperty(
            "Conditions",
            extension.conditions?.length ?? 0
        )}
                        </dl>
                    </uui-box>

                    <uui-box headline="Complete manifest">
                        <pre>${this.stringify(extension)}</pre>
                    </uui-box>

                </div>

                <div slot="actions">
                    <uui-button
                        label="Close"
                        look="primary"
                        @click=${this.close}>
                        Close
                    </uui-button>
                </div>

            </umb-body-layout>
        `;
    }

    renderProperty(label, value) {
        return html`
            <div class="property">
                <dt>${label}</dt>
                <dd>${value ?? "—"}</dd>
            </div>
        `;
    }

    stringify(value) {
        try {
            return JSON.stringify(
                value,
                (key, item) => {
                    if (typeof item === "function") {
                        return `[Function: ${item.name || "anonymous"}]`;
                    }

                    return item;
                },
                2
            );
        }
        catch {
            return "Unable to serialize manifest.";
        }
    }

    close() {
        this.modalContext?.reject();
    }

    static styles = css`
        :host {
            display: block;
        }

        .content {
            display: grid;
            gap: var(--uui-size-space-5);
            padding: var(--uui-size-layout-1);
        }

        dl {
            margin: 0;
        }

        .property {
            display: grid;
            grid-template-columns: 160px 1fr;
            gap: var(--uui-size-space-4);
            padding: var(--uui-size-space-3) 0;
            border-bottom:
                1px solid var(--uui-color-border);
        }

        .property:last-child {
            border-bottom: 0;
        }

        dt {
            font-weight: 700;
        }

        dd {
            margin: 0;
            overflow-wrap: anywhere;
        }

        pre {
            margin: 0;
            padding: var(--uui-size-space-4);
            background: var(--uui-color-surface-alt);
            border-radius: var(--uui-border-radius);
            overflow: auto;
            font-family: monospace;
            font-size: 13px;
            line-height: 1.5;
            white-space: pre-wrap;
            overflow-wrap: anywhere;
        }
    `;
}

customElements.define("my-extension-details-modal", ExtensionDetailsModalElement);

export default ExtensionDetailsModalElement;
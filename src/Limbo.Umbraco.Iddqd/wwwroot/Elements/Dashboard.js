import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";

export class LimboIddqdDashboardElement extends UmbElementMixin(LitElement) {

    constructor() {
        super();
    }

    render() {
        return html`
            <div>Hello there!</div>
        `;
    }

}

customElements.define("limbo-iddqd-dashboard", LimboIddqdDashboardElement);

export default LimboIddqdDashboardElement;
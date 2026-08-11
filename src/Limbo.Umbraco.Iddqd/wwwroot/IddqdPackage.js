export class IddqdPackage {

    static set serverVariables(value) {
        this._serverVariables = value;
    }

    static get version() {
        return this._serverVariables["version"];
    }

    static get cacheBuster() {
        return this._serverVariables["cacheBuster"];
    }

}

const DELAY = 250;

export async function delay(ms) {
    return new Promise((resolve) => setTimeout(resolve, ms));
}

export async function delayed(callback) {
    const [, result] = await Promise.all([delay(DELAY), callback()]);
    return result;
}

export default IddqdPackage;
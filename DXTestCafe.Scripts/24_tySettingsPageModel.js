import { Selector, t } from "testcafe";

class SettingsPageModel {
    constructor() {
        this.profilePictureInput = Selector("#profile_picture_input");
        this.clearPictureButton = Selector("#clear_profile_picture_btn");
    }
}
export default new SettingsPageModel();



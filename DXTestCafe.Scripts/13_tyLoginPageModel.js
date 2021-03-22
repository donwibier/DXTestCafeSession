import { Selector } from "testcafe";

class LoginPageModel {
    constructor() {
        this.emailField = Selector("#Input_Email");
        this.passwordField = Selector("#Input_Password");
        this.loginSubmitButton = Selector("#login-submit");
        this.logginUserContainer = Selector("#loggedin-user");
    }
}

export default new LoginPageModel();
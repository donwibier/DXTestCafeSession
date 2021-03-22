import loginPageModel from "./13_tyLoginPageModel";

fixture("Login")
    .page("https://localhost:44382/Identity/Account/Login");

    test("User with valid account can log in", async t => {
        await t
            .typeText(loginPageModel.emailField, "admin@localhost")
            .typeText(loginPageModel.passwordField, "Test123$");

        await t.click(loginPageModel.loginSubmitButton);
        
        await t
            .expect(loginPageModel.logginUserContainer.innerText)
            .contains("admin@localhost");

});
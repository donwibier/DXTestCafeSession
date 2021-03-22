import { Selector } from "testcafe";

fixture("My Thoughts App")
    .page("https://localhost:44382/Identity/Account/Login");


    test("Logged-in user can create new feed post", async t => {
        await t
            .typeText("#Input_Email", "admin@localhost")
            .typeText("#Input_Password", "Test123$")
            .click("#login-submit");

        await t
            .typeText('#TitleCtrl .dx-texteditor-input', 'Something else')
            .typeText('#BodyCtrl .dx-texteditor-input', 'What do you think about NDC London ?')
            .typeText('#PostDateCtrl .dx-texteditor-input', '1/29/2021')
            .click("#PostBtn");

        await t
            .expect(Selector("#reactions-box").nth(0).innerText)
                .contains("Something else");
    });

    test("Logged-in user can comment on feed post", async t => {
        await t
            .typeText("#Input_Email", "don@localhost")
            .typeText("#Input_Password", "Test456&")
            .click("#login-submit");

        await t
            .navigateTo("https://localhost:44382/details/admin/something-else")
            .typeText("#ContentCtrl textarea", "All NDC Conferences are awesome")
            .click("#SubmitBtn");
    });

    test("Admin can activate on feed comment", async t => {

        await t
            .typeText("#Input_Email", "admin@localhost")
            .typeText("#Input_Password", "Test123$")
            .click("#login-submit");

        await t
            .navigateTo("https://localhost:44382/details/admin/something-else");

        
        await t
            .click(Selector("form .dx-switch").nth(0));

        await t
            .expect(Selector(".comment-text").nth(0).innerText)
                .contains("All NDC Conferences are awesome");


    });
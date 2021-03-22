import { Selector } from "testcafe";

fixture("My Thoughts App")
    .page("https://localhost:44382/Identity/Account/Login")
    .beforeEach(async t => {
        await t
            .typeText("#Input_Email", "admin@localhost")
            .typeText("#Input_Password", "Test123$")
            .click("#login-submit");
    });
    
    const title = "TestCafe OpenSource"
    const body = "Did you know you can use TestCafe OpenSource for free?"
    const postDate = '1/29/2021';
    
    test("Logged-in user can create new feed post", async t => {

        await t
            .typeText('#TitleCtrl .dx-texteditor-input', title)
            .typeText('#BodyCtrl .dx-texteditor-input', body)
            .typeText('#PostDateCtrl .dx-texteditor-input', postDate)
            .click("#PostBtn");

        await t
            .expect(Selector("#reactions-box").nth(0).innerText)
                .contains(title);
        });


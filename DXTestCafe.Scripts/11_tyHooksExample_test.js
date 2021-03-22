import { Selector } from "testcafe";
import { adminUser } from "./10_tyRoles";

fixture("TeamYap Feed")
    .page("https://localhost:44382/Identity/Account/Login")
    .beforeEach(async t => {
        await t.useRole(adminUser);
    });
        
    const title = "TestCafe Studio"
    const body = "Did you know there is also a TestCafe Studio?"
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

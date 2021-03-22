import { Selector } from 'testcafe';

fixture`A set of examples that illustrate how to use TestCafe API`
    .page`http://devexpress.github.io/testcafe/example/`;


const developerName = Selector('#developer-name');
const triedLabel    = Selector('label').withText('I have tried TestCafe');
const sliderHandle  = Selector('#slider').child('span');
const submitButton  = Selector('#submit-button');

test('How to type text into an input (t.typeText user action)', async t => {
    await t
        .typeText(developerName, 'Peter')
        .typeText(developerName, 'Paker', { replace: true })
        .typeText(developerName, 'r', { caretPos: 2 })
        .expect(developerName.value).eql('Parker');
});

test('How to click check boxes and then verify their states (t.click user action and ok() assertion)', async t => {
    await t
        .click(Selector('label').withText('Support for testing on remote devices'))
        .click(Selector('label').withText('Re-using existing JavaScript code for testing'))
        .expect(Selector('#remote-testing').checked).ok()
        .expect(Selector('#reusing-js-code').checked).ok();
});

test('How to type text into an input, click at a specified position and backspace a symbol (t.typeText, t.click, t.pressKey user actions and eql() assertion)', async t => {
    await t
        .typeText(developerName, 'Peter Parker')
        .click(developerName, { caretPos: 5 })
        .pressKey('backspace')
        .expect(developerName.value).eql('Pete Parker');
});

test('How to press a specified key (t.pressKey user action)', async t => {
    await t
        .typeText(developerName, 'Peter Parker')
        .pressKey('home right . delete delete delete delete')
        .expect(developerName.value).eql('P. Parker');
});

test('How to implement comparison (ok() and notOk() assertions)', async t => {
    const comments = Selector('#comments');

    await t
        .expect(comments.hasAttribute('disabled')).ok()
        .click(triedLabel)
        .expect(comments.hasAttribute('disabled')).notOk();
});

test('How to compare a value in an input with an expected one (eql() assertion)', async t => {
    await t
        .typeText(developerName, 'Peter Parker')
        .click(submitButton)
        .expect(Selector('#article-header').textContent).eql('Thank you, Peter Parker!');
});

test('How to hover over an element (t.hover user action)', async t => {
    await t
        .click(triedLabel)
        .expect(sliderHandle.hasClass('ui-state-hover')).notOk()
        .hover(sliderHandle)
        .expect(sliderHandle.hasClass('ui-state-hover')).ok();
});

test('How to select text in an input (t.selectText user action)', async t => {
    await t
        .typeText(developerName, 'Test Cafe')
        .selectText(developerName, 7, 1)
        .pressKey('delete')
        .expect(developerName.value).eql('Tfe');
});

test('How to handle a confirmation dialog (t.setNativeDialogHandler() and t.getNativeDialogHistory())', async t => {
    await t
        .setNativeDialogHandler(() => false)
        .click('#populate')
        .expect(developerName.value).eql('')

        .setNativeDialogHandler(() => true)
        .click('#populate')
        .expect(developerName.value).eql('Peter Parker');

    const dialogHistory = await t.getNativeDialogHistory();

    await t
        .expect(dialogHistory[0].type).eql('confirm')
        .expect(dialogHistory[1].type).eql('confirm');
});

test('A simple test on our example page', async t => {
    const interfaceSelect = Selector('#preferred-interface');

    await t
        .click(Selector('label').withText('MacOS'))
        .typeText(developerName, 'Peter Parker')
        .click(interfaceSelect)
        .click(interfaceSelect.find('option').withText('Both'))
        .click(submitButton)
        .expect(Selector('#article-header').textContent).eql('Thank you, Peter Parker!');
});
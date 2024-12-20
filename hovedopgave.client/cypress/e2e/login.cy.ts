/// <reference types="cypress" />

describe('Login Functionality', () => {
  beforeEach(() => {
    cy.visit('https://localhost:4200/login');
  });

  it('should log in successfully with valid credentials', () => {
    const username = 'Faker';
    const password = '1234';

    cy.get('input[id="username"]').type(username);
    cy.get('input[id="password"]').type(password);

    // Mock login request
    cy.intercept('POST', 'https://localhost:7213/login', {
      statusCode: 200,
      body: { token: 'mockToken' },
    }).as('loginRequest');

    cy.get('button[id="loginbutton"]').click();

    // Assert successful login
    cy.wait('@loginRequest').its('response.statusCode').should('eq', 200);
    cy.url().should('include', '/dashboard');
  });

  it('should display an error for invalid credentials', () => {
    const username = 'InvalidUser';
    const password = 'InvalidPassword';

    cy.get('input[id="username"]').type(username);
    cy.get('input[id="password"]').type(password);

    cy.intercept('POST', 'https://localhost:7213/login', {
      statusCode: 401,
      body: { message: 'Login failed. Please try again.' },
    }).as('loginRequest');

    cy.get('button[id="loginbutton"]').click();

    // Assert error message
    cy.wait('@loginRequest').its('response.statusCode').should('eq', 401);

    // Adjust to avoid space-related issues
    cy.get('#error-message')
      .invoke('text')
      .then((text) => {
        expect(text.trim()).to.equal('Login failed. Please try again.');
      });
  });
});

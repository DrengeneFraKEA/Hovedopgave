/// <reference types="cypress" />

/* e2e tests covered here:
should display total statistics in the dashboard overviewpassed
should navigate to the Users statistics viewpassed
should navigate to the Teams statistics viewpassed
should navigate to the Organizations statistics viewpassed
should filter by daily, weekly, and monthly viewspassed
should navigate to the Admin Rights view and search for a userpassed
should open the edit modal for a user in Admin Rights viewpassed
should apply a custom date range filter
*/

describe('Dashboard and Sidebar Functionality', () => {
  beforeEach(() => {
    // Visit the login page and log in
    cy.visit('https://localhost:4200/login');

    const username = 'Faker';
    const password = '1234';

    cy.get('input[id="username"]').type(username);
    cy.get('input[id="password"]').type(password);

    // Click login and go to /main
    cy.get('button[id="loginbutton"]').click();

    cy.request('POST', 'https://localhost:7213/login', { username: username, password: password }).then(({ body }) => {
      expect(body.user_id).eq('usr_01JBC2KQ4SAV6SW2QDC6DKSKD7'); // User logged in correctly?
      expect(body.token).to.not.equal(""); // Is token legitimate?
    });
    
    /* Mock the login response
    cy.intercept('POST', 'https://localhost:7213/login', {
      statusCode: 200,
      body: { token: 'mockToken' },
    }).as('loginRequest');
    */
    // cy.get('input[id="username"]').type(username);
    //  cy.get('input[id="password"]').type(password);
    //cy.get('button[id="loginbutton"]').click();
  })

  

  it('should display total statistics in the dashboard overview', () => {
    cy.get('.sidebar').trigger('mouseover'); // Ensure sidebar is hovered
    cy.get('#nav-overview').click({ force: true }); // Force click
    cy.get('.stats').should('exist'); // Check stats section
    cy.get('.stat-title').contains('Total Users').should('exist'); // Assert Total Users stat box
    cy.get('.stat-title').contains('Total Teams').should('exist'); // Assert Total Teams stat box
    cy.get('.stat-title').contains('Total Organizations').should('exist'); // Assert Total Organizations stat box
  });

  it('should navigate to the Users statistics view', () => {
    cy.get('.sidebar').trigger('mouseover'); 
    cy.get('a').contains('Users').click({ force: true }); //  Users
    cy.get('canvas#chart-container').should('exist'); // Assert chart exists
  });

  it('should navigate to the Teams statistics view', () => {
    cy.get('.sidebar').trigger('mouseover'); 
    cy.get('a').contains('Teams').click({ force: true }); // Teams
    cy.get('canvas#chart-container').should('exist'); // Assert chart exists
  });

  it('should navigate to the Organizations statistics view', () => {
    cy.get('.sidebar').trigger('mouseover'); 
    cy.get('a').contains('Organizations').click({ force: true }); //  Organizations
    cy.get('canvas#chart-container').should('exist'); // Assert chart exists
  });

  it('should filter by daily, weekly, and monthly views', () => {
    cy.get('.sidebar').trigger('mouseover'); // 
    cy.get('#nav-overview').click({ force: true }); // Overview

    // Daily filter
    cy.get('button').contains('Daily').click();
    cy.get('canvas#chart-container').should('exist'); // Assert chart exists for daily

    // Weekly filter
    cy.get('button').contains('Weekly').click();
    cy.get('canvas#chart-container').should('exist'); // Assert chart exists for weekly

    // Monthly filter
    cy.get('button').contains('Monthly').click();
    cy.get('canvas#chart-container').should('exist'); // Assert chart exists for monthly
  });

  it('should navigate to the Admin Rights view and search for a user', () => {
    // Hover over sidebar and go to Admin Rights
    cy.get('.sidebar').trigger('mouseover');
    cy.get('#nav-adminrights').click({ force: true });

    //https://docs.cypress.io/api/commands/intercept "Spy and stub network requests/responses"
    // Intercept the GET request that runs when typing "Faker"
    cy.intercept('GET', '**/adminrights/search-users/Faker*').as('searchUsersFaker');

    // Type "Faker" into the search box
    cy.get('#search-users-input').type('Faker');

    // Wait for request before assert
    cy.wait('@searchUsersFaker');

    // 4) Assert "Faker" shows up
    cy.contains('span.flex-1', 'Faker', { timeout: 10000 }).should('be.visible');
  });



  it('should open the edit modal for a user in Admin Rights view', () => {
    // Hover over sidebar and go to Admin Rights
    cy.get('.sidebar').trigger('mouseover');
    cy.get('#nav-adminrights').click({ force: true });

    //https://docs.cypress.io/api/commands/intercept "Spy and stub network requests/responses"
    cy.intercept('GET', '**/adminrights/search-users/Faker*').as('searchAdminUser');

    cy.get('#search-users-input').type('Faker');

    // Wait for request
    cy.wait('@searchAdminUser');

    // click the Edit button
    cy.contains('span.flex-1', 'Faker', { timeout: 10000 })
      .parent() // the .flex items-center row
      .find('button')
      .contains('Edit')
      .click();

    //  Check the modal is visible.
    cy.get('#admin-modal').should('have.class', 'modal-open');
  });


  it('should apply a custom date range filter', () => {
    cy.get('.sidebar').trigger('mouseover'); 
    cy.get('#nav-overview').click({ force: true }); 

    // Set custom date range
    cy.get('#fromDate').type('2023-01-01');
    cy.get('#toDate').type('2023-12-31');
    cy.get('button').contains('Custom').click();

    // Assert chart reflects the date range
    cy.get('canvas#chart-container').should('exist');
  });
});

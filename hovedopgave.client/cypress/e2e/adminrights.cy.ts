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

    // Mock the login response
    cy.intercept('POST', 'https://localhost:7213/login', {
      statusCode: 200,
      body: { token: 'mockToken' },
    }).as('loginRequest');

    cy.get('input[id="username"]').type(username);
    cy.get('input[id="password"]').type(password);
    cy.get('button[id="loginbutton"]').click();
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
    cy.get('.sidebar').trigger('mouseover'); // Hover over sidebar
    cy.get('a').contains('Users').click({ force: true }); // Navigate to Users
    cy.get('canvas#chart-container').should('exist'); // Assert chart exists
  });

  it('should navigate to the Teams statistics view', () => {
    cy.get('.sidebar').trigger('mouseover'); // Hover over sidebar
    cy.get('a').contains('Teams').click({ force: true }); // Navigate to Teams
    cy.get('canvas#chart-container').should('exist'); // Assert chart exists
  });

  it('should navigate to the Organizations statistics view', () => {
    cy.get('.sidebar').trigger('mouseover'); // Hover over sidebar
    cy.get('a').contains('Organizations').click({ force: true }); // Navigate to Organizations
    cy.get('canvas#chart-container').should('exist'); // Assert chart exists
  });

  it('should filter by daily, weekly, and monthly views', () => {
    cy.get('.sidebar').trigger('mouseover'); // Hover over sidebar
    cy.get('#nav-overview').click({ force: true }); // Navigate to Overview

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
    cy.get('.sidebar').trigger('mouseover'); // Hover over sidebar
    cy.get('#nav-adminrights').click({ force: true }); // Navigate to Admin Rights

    // Search for a user
    cy.get('input#search-users-input').type('Faker'); // Enter a search query
    cy.get('.user-display-name').contains('Faker').should('exist'); // Assert user is found
  });

  it('should open the edit modal for a user in Admin Rights view', () => {
    cy.get('.sidebar').trigger('mouseover'); // Hover over sidebar
    cy.get('#nav-adminrights').click({ force: true }); // Navigate to Admin Rights

    // Search and open the edit modal
    cy.get('input#search-users-input').type('AdminUser'); // Search for an admin
    cy.get('button').contains('Edit').click(); // Click the Edit button
    cy.get('#admin-modal').should('have.class', 'active'); // Assert modal is active
  });



  it('should apply a custom date range filter', () => {
    cy.get('.sidebar').trigger('mouseover'); // Hover over sidebar
    cy.get('#nav-overview').click({ force: true }); // Navigate to Overview

    // Set a custom date range
    cy.get('#fromDate').type('2023-01-01');
    cy.get('#toDate').type('2023-12-31');
    cy.get('button').contains('Custom').click();

    // Assert chart reflects the date range
    cy.get('canvas#chart-container').should('exist');
  });
});

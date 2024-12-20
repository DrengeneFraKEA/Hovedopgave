describe('Navigation Functionality', () => {
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

    // Wait for login to complete
    cy.wait('@loginRequest').its('response.statusCode').should('eq', 200);

    // Visit the dashboard after login
    cy.visit('https://localhost:4200/dashboard');

    // Wait for the dashboard stats request
    cy.intercept('GET', 'https://localhost:7213/statistics/totals/overview').as('loadStats');


    // Ensure sidebar is present
    cy.get('app-sidebar', { timeout: 10000 }).should('exist');
    cy.get('.sidebar', { timeout: 5000 }).trigger('mouseover');
  });

  it('should navigate to the Dashboard Overview', () => {
    cy.get('.sidebar').trigger('mouseover'); // Ensure sidebar is hovered
    cy.get('#nav-overview').click({ force: true }); // Force click
    cy.get('.dashboard-stats').should('exist'); // Assert the stats section is visible
    cy.get('.stat-box h3').contains('Total Users').should('exist'); // Assert a specific stat box
  });

  it('should navigate to the Admin Rights view', () => {
    cy.get('.sidebar').trigger('mouseover'); // Ensure sidebar is hovered
    cy.get('#nav-adminrights').click({ force: true }); // Force click
    cy.get('app-adminrights').should('exist'); // Assert the admin rights component is loaded
  });
});




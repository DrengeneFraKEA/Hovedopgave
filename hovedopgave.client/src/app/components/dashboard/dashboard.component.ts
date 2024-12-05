import { Component, OnInit } from '@angular/core';
import { StatisticsService, SignupStats } from '../../services/Statistics.service';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  stats: SignupStats = {  //Default values, for at undgå fejl ved 0 data
    totalSignups: 0,
    userSignups: 0,
    teamSignups: 0,
    organizationSignups: 0,
    dailySignups: 0,
    weeklySignups: 0,
    monthlySignups: 0
  }; 

  selectedFilter: string = 'daily'; // Indicates the active filter (daily, weekly, monthly)
  selectedView: string = 'users';  
  fromDate: string | null = null; 
  toDate: string | null = null;   

  //Til dato periode 
  filter = {
    fromDate: '',
    toDate: ''
  };

  constructor(private http: HttpClient, private route: Router, private statisticsService: StatisticsService) { }

  ngOnInit() {
    var token = localStorage.getItem("token");
    if (token === null || token === "") this.route.navigate(['login']);
    this.fetchStats();
  }

  setFilter(filter: string) {
    this.selectedFilter = filter;
    this.fromDate = null;
    this.toDate = null;
    this.fetchStats();
  }

  navigateTo(view: string) {
    this.selectedView = view;
    this.fetchStats();
  }

  // Fetches the current filter, selected view, or custom date range
  fetchStats() {
    if (this.fromDate && this.toDate) {
      this.statisticsService.getSignupStats(this.fromDate, this.toDate).subscribe({
        next: (data) => {
          this.stats = data;
        },
        error: (error) => {
          console.error('Error fetching stats:', error);
        }
      });
    } else {
      //Stat filters
      const now = new Date();
      switch (this.selectedFilter) {
        case 'daily':
          this.fromDate = this.toDate = now.toISOString().split('T')[0];
          break;
        case 'weekly':
          this.fromDate = new Date(now.setDate(now.getDate() - 7)).toISOString().split('T')[0];
          this.toDate = new Date().toISOString().split('T')[0];
          break;
        case 'monthly':
          this.fromDate = new Date(now.setMonth(now.getMonth() - 1)).toISOString().split('T')[0];
          this.toDate = new Date().toISOString().split('T')[0];
          break;
      }
      this.statisticsService.getSignupStats(this.fromDate, this.toDate).subscribe({
        next: (data) => {
          this.stats = data;
        },
        error: (error) => {
          console.error('Error fetching stats:', error);
        }
      });
    }
  }

  applyDateRange(from: string, to: string) {
    this.fromDate = from;
    this.toDate = to;
    this.selectedFilter = ''; 
    this.fetchStats();
  }

  // Exports the currently displayed data to a CSV file
  exportToCSV() {
    const headers = ['Metric', 'Value'];
    const rows = [
      ['Total Signups', this.stats.totalSignups],
      ['User Signups', this.stats.userSignups],
      ['Team Signups', this.stats.teamSignups],
      ['Organization Signups', this.stats.organizationSignups],
      ['Daily Signups', this.stats.dailySignups],
      ['Weekly Signups', this.stats.weeklySignups],
      ['Monthly Signups', this.stats.monthlySignups]
    ];

    // Generate CSV content
    const csvContent = [
      headers.join(','), // Add headers
      ...rows.map(row => row.join(',')) // Add each row
    ].join('\n');

    // Create a Blob with the CSV content
    const blob = new Blob([csvContent], { type: 'text/csv' });

    // Create an object URL and download it
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = `signup_statistics_${new Date().toISOString()}.csv`;
    link.click();

    // Clean up URL object
    window.URL.revokeObjectURL(link.href);
  }
}

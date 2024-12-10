import { Component, OnInit } from '@angular/core';
import { StatisticsService, SignupStats } from '../../services/Statistics.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

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
  totalUserSignups: number = 0;
  totalTeamSignups: number = 0;
  totalOrganizationSignups: number = 0;
  totalValorantProfiles: number = 0;
  totalUserGameProfiles: number = 0;
  totalLeagueProfiles: number = 0;
  totalCompetitions: number = 0;

  selectedFilter: string = 'weekly';
  selectedView: string = 'overview';
  fromDate: string | null = null;
  toDate: string | null = null;

  //Til dato periode 
  filter = {
    fromDate: '',
    toDate: ''
  };

  cache: { [key: string]: number } = {}; // Cache for fetched totals
  constructor(private http: HttpClient, private route: Router, private statisticsService: StatisticsService) { }

  ngOnInit() {
    const token = localStorage.getItem('token');
    if (!token) this.route.navigate(['login']);
    this.updateView('overview'); // Default view
    this.fetchStats();
  }

  updateView(view: string) {
    this.selectedView = view;

    if (view === 'overview') {
      this.fetchOverviewTotals();
    } else {
      this.fetchStats();
    }
  }

  fetchOverviewTotals() {
    this.statisticsService.getOverviewTotals().subscribe({
      next: (data) => {
        this.totalUserSignups = data.totalUsers;
        this.totalTeamSignups = data.totalTeams;
        this.totalOrganizationSignups = data.totalOrganizations;
        this.totalValorantProfiles = data.totalValorantProfiles;
        this.totalUserGameProfiles = data.totalUserGameProfiles;
        this.totalLeagueProfiles = data.totalLeagueProfiles;
        this.totalCompetitions = data.totalCompetitions;
      },
      error: (error) => console.error('Error fetching overview totals:', error)
    });
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


 
  setFilter(filter: string) {
    this.selectedFilter = filter;
    this.fromDate = null;
    this.toDate = null;
    this.fetchStats();
  }
  
}

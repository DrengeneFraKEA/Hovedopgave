import { Component, OnInit } from '@angular/core';
import { StatisticsService } from '../../services/Statistics.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  totalUserSignups: number = 0;
  totalTeamSignups: number = 0;
  totalOrganizationSignups: number = 0;

  fromDate: string | null = null;
  toDate: string | null = null;
  //selectedFilter: string = 'weekly';
  selectedView: string = 'overview';

  cache: { [key: string]: number } = {}; // Cache for fetched totals
  constructor(private statisticsService: StatisticsService, private route: Router) { }

  ngOnInit() {
    const token = localStorage.getItem('token');
    if (!token) this.route.navigate(['login']);
    this.updateView('overview'); // Default view
  }

  updateView(view: string) {
    this.selectedView = view;

    if (view === 'overview') {
      this.fetchTotals();
    }
  }

  fetchTotals() {
    // Use cached data if available
    if (this.cache['users'] !== undefined) {
      this.totalUserSignups = this.cache['users'];
      this.totalTeamSignups = this.cache['teams'];
      this.totalOrganizationSignups = this.cache['organizations'];
      return;
    }

    this.statisticsService.getTotalUsers().subscribe({
      next: (totalUsers) => {
        this.totalUserSignups = totalUsers;
        this.cache['users'] = totalUsers;
      },
      error: (error) => console.error('Error fetching total users:', error)
    });

    this.statisticsService.getTotalTeams().subscribe({
      next: (totalTeams) => {
        this.totalTeamSignups = totalTeams;
        this.cache['teams'] = totalTeams;
      },
      error: (error) => console.error('Error fetching total teams:', error)
    });

    this.statisticsService.getTotalOrganizations().subscribe({
      next: (totalOrganizations) => {
        this.totalOrganizationSignups = totalOrganizations;
        this.cache['organizations'] = totalOrganizations;
      },
      error: (error) => console.error('Error fetching total organizations:', error)
    });
  }

  /*
  fetchStats() {
    const fromDate = this.fromDate || undefined;
    const toDate = this.toDate || undefined;

    this.statisticsService.getStats(this.selectedView, fromDate, toDate).subscribe({
      next: (data) => {
        this.currentStats = data;
      },
      error: (error) => {
        console.error('Error fetching stats:', error);
      }
    });
  }

  calculateTotal(stats: { [key: string]: number }): number {
    return Object.values(stats).reduce((total, count) => total + count, 0);
  }

  onDateChange(event: Event, field: 'fromDate' | 'toDate') {
    const input = event.target as HTMLInputElement;
    if (field === 'fromDate') {
      this.fromDate = input.value || null;
    } else if (field === 'toDate') {
      this.toDate = input.value || null;
    }
    this.fetchStats();
  }
  */

 /*
  setFilter(filter: string) {
    this.selectedFilter = filter;

    const now = new Date();
    switch (filter) {
      case 'daily':
        this.fromDate = now.toISOString().split('T')[0];
        this.toDate = now.toISOString().split('T')[0];
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

    this.fetchStats();
  }

  setView(view: string) {
    this.selectedView = view;
    this.fetchStats();
  }

  getCurrentStatsKeys(): string[] {
    return Object.keys(this.currentStats || {});
  }
  */
}

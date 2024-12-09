import { Component, OnInit } from '@angular/core';
import { StatisticsService, SignupStats } from '../../services/Statistics.service';
import { graphData, GraphService } from '../../services/graph.service';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  constructor(private http: HttpClient, private route: Router, private statisticsService: StatisticsService, private graphService: GraphService) { }

  stats: SignupStats = {
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

  filter = {
    fromDate: '',
    toDate: ''
  };

  data: graphData[] = [];
  cache: { [key: string]: number } = {};

  ngOnInit() {
    const token = localStorage.getItem('token');
    if (!token) this.route.navigate(['login']);
    this.updateView('overview'); // Default view
    this.fetchStats();
  }

  setFilter(filter: string) {
    this.selectedFilter = filter;
    this.fromDate = null;
    this.toDate = null;
    //this.fetchStats();
    this.RefreshGraphData();
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

  RefreshGraphData() {
    this.http.get<graphData[]>(`https://localhost:7213/graph/${this.selectedFilter}/${this.selectedView}`).subscribe((data) => {
      this.data = data;
      this.DrawLineChart();
    });
  }

  DrawLineChart() {
    const labels = this.data.map(item => item.date);
    const values = this.data.map(item => item.value);

    if (Chart.getChart("chart-container")) {
      Chart.getChart("chart-container")?.destroy();
    }

    const ctx = document.getElementById('chart-container') as HTMLCanvasElement;
    new Chart(ctx, {
      type: 'line',
      data: {
        labels: labels,
        datasets: [{
          label: `${this.selectedFilter} registered ${this.selectedView}`,
          data: values,
          borderColor: 'rgba(55, 44, 200, 1)',
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
          borderWidth: 3,
          tension: 0
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            position: 'top',
          },
          tooltip: {
            callbacks: {
              label: function (tooltipItem) {
                return tooltipItem.raw + " registrations";
              }
            }
          }
        },
        scales: {
          x: {
            title: {
              display: true,
              text: 'Date'
            }
          },
          y: {
            title: {
              display: true,
              text: 'Registrations'
            },
            beginAtZero: true
          }
        }
      }
    });
  }
}

import { Component, OnInit } from '@angular/core';
import { StatisticsService, SignupStats } from '../../services/Statistics.service';
import { graphData, GraphService } from '../../services/graph.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
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

  selectedFilter: string = '';
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
    this.updateView('overview');
    this.statisticsService.getOverviewTotals().subscribe((data) =>
    {
      this.totalUserSignups = data.totalUsers;
      this.totalTeamSignups = data.totalTeams;
      this.totalOrganizationSignups = data.totalOrganizations;
    });
  }

  setFilter(filter: string) {
    this.selectedFilter = filter;
    this.RefreshGraphData();
  }

  updateView(view: string) {
    this.selectedView = view;
    this.selectedFilter = '';
    this.clearGraph();
  }

  applyDateRangeChange(from: string, to: string) {
    this.fromDate = from;
    this.toDate = to;
    this.selectedFilter = '';
  }

  CustomRefreshGraphData()
  {
    const params = new HttpParams().append('fromDate', `${this.fromDate}`).append('toDate', `${this.toDate}`);
    this.selectedFilter = 'custom';
    this.http.get<graphData[]>(`https://localhost:7213/graph/custom/${this.selectedView}`, { params }).subscribe((data) => {
      this.data = data;
      this.DrawLineChart();
    });
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

  clearGraph() {
    this.data = [];
    if (Chart.getChart("chart-container")) {
      Chart.getChart("chart-container")?.destroy();
    }
  }
}

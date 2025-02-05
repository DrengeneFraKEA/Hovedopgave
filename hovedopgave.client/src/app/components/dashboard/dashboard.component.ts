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
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('token')
    });

    const params = new HttpParams()
      .set('fromDate', `${this.fromDate}`)
      .set('toDate', `${this.toDate}`)
      .set('type', `${this.selectedView}`);

    this.selectedFilter = 'custom';
  
    this.http.get<graphData[]>('https://localhost:7213/graph/custom', { headers, params })
      .subscribe((data) => {
        this.data = data;
        this.DrawLineChart();
      });
  }

  RefreshGraphData() {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      })
    };
    this.http.get<graphData[]>(`https://localhost:7213/graph/${this.selectedFilter}/${this.selectedView}`, httpOptions).subscribe((data) => {

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

    const canvas = document.getElementById('chart-container') as HTMLCanvasElement;

    // Get device pixel ratio
    const devicePixelRatio = window.devicePixelRatio || 1;
    const width = 1700;
    const height = 1000;

    // Set the display size of the canvas
    canvas.style.width = `${width}px`;
    canvas.style.height = `${height}px`;

    // Adjust the canvas internal resolution for higher DPI screens
    canvas.width = width * devicePixelRatio;
    canvas.height = height * devicePixelRatio;


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
        responsive: false,
        maintainAspectRatio: false,
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

  exportToCSV() {
    if (!this.data || this.data.length === 0) {
      alert("No data available to export!");
      return;
    }

    const csvHeader = "Date,Count\n"; 
    const csvRows = this.data.map(row => `${row.date},${row.value}`).join("\n"); // Convert data rows to CSV format
    const csvContent = csvHeader + csvRows;

    // Create a Blob with the CSV data
    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });

    // Create a download link
    const link = document.createElement("a");
    const url = URL.createObjectURL(blob);
    link.setAttribute("href", url);
    link.setAttribute("download", `${this.selectedView}_${this.selectedFilter || 'custom'}.csv`);
    document.body.appendChild(link);

    link.click();

    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  }

}

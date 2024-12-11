import { Component, OnInit } from '@angular/core';
import { StatisticsService, SignupStats } from '../../services/Statistics.service';
import { graphData, GraphService } from '../../services/graph.service';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import Chart from 'chart.js/auto'


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {


  constructor(private http: HttpClient, private route: Router, private statisticsService: StatisticsService, private graphService: GraphService) { }

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

  data: graphData[] = [];

  ngOnInit() {
    var token = localStorage.getItem("token");
    if (token === null || token === "") this.route.navigate(['login']);
    this.fetchStats();
    // this.DrawLineChart();
  }

  setFilter(filter: string) {
    this.selectedFilter = filter;
    this.fromDate = null;
    this.toDate = null;
    this.fetchStats();
    this.RefreshGraphData();
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

  RefreshGraphData()
  {
    this.http.get<graphData[]>(`https://localhost:7213/graph/${this.selectedFilter}/${this.selectedView}`).subscribe((data) =>
    {
      this.data = data
      this.DrawLineChart();
    });
  }


  DrawLineChart()
  {
    // Prepare the chart data
    const labels = this.data.map(item => item.date);
    const values = this.data.map(item => item.value);


    if (Chart.getChart("chart-container")) {
      Chart.getChart("chart-container")?.destroy()
    }

    // Create the chart
    const ctx = document.getElementById('chart-container') as HTMLCanvasElement; // Get the canvas element by ID
    new Chart(ctx, {
      type: 'line',
      data: {
        labels: labels,
        datasets: [{
          label: `${this.selectedFilter} registered ${this.selectedView}`,
          data: values,
          borderColor: 'rgba(55, 44, 200, 1)', // Line color
          backgroundColor: 'rgba(75, 192, 192, 0.2)', // Fill color
          borderWidth: 3,
          tension: 0 // For smooth lines
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
                return tooltipItem.raw + " registrations"; // Custom tooltip label
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

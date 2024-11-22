import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  stats = {
    totalUsers: 0,
    totalMatches: 0,
    totalTeams: 0
  };
  selectedFilter: string = 'daily';
  selectedView: string = 'users';

  constructor(private http: HttpClient, private route: Router) { }

  ngOnInit() {
    var token = localStorage.getItem("token");
    if (token === null || token === "") this.route.navigate(['login']);
    this.fetchStats();
  }

  setFilter(filter: string) {
    this.selectedFilter = filter;
    this.fetchStats();
  }

  navigateTo(view: string) {
    this.selectedView = view;
    this.fetchStats();
  }

  fetchStats() {
    const params = {
      filter: this.selectedFilter,
      view: this.selectedView
    };

    this.http.get<any>('/api/dashboard/stats', { params }).subscribe(
      (data) => {
        this.stats = data;
      },
      (error) => {
        console.error('Error fetching stats:', error);
      }
    );
  }
}

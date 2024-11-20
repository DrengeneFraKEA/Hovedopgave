import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './component/login/login.component';
import { AdminrightsComponent } from './components/adminrights/adminrights.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'adminrights', component: AdminrightsComponent },
  { path: 'dashboard', component: DashboardComponent },
 // { path: 'dashboard/users', component: UsersComponent }, Til forskellige dashboard views senere hen
 // { path: 'dashboard/matches', component: MatchesComponent },
 // { path: 'dashboard/teams', component: TeamsComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

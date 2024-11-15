import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminrightsComponent } from './components/adminrights/adminrights.component';
import { LoginComponent } from './components/login/login.component';

const routes: Routes = [
  { path: 'adminrights', component: AdminrightsComponent },
  { path: 'login', component: LoginComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

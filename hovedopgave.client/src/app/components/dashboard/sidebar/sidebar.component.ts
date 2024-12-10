import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {
  collapsed: boolean = false;

  @Output() navigate = new EventEmitter<string>();

  updateView(view: string) {
    this.navigate.emit(view); 
  }
  toggleSidebar() {
    this.collapsed = !this.collapsed;
  }
}

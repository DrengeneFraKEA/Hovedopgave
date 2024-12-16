import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {

  @Output() navigate = new EventEmitter<string>();

  updateView(view: string) {
    this.navigate.emit(view); 
  }
}

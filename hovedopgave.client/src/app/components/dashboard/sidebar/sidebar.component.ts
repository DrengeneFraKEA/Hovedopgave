import { Component, EventEmitter, Output, Renderer2 } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {

  @Output() navigate = new EventEmitter<string>();
  currentTheme: string = 'light';
  activeView: string = 'overview';

  constructor(private renderer: Renderer2) { }

  updateView(view: string) {
    this.activeView = view;
    console.log("active view: " + this.activeView);
    this.navigate.emit(view);
  }

  toggleTheme(event: Event) {
    const isChecked = (event.target as HTMLInputElement).checked;
    this.currentTheme = isChecked ? 'dark' : 'light';
    this.renderer.setAttribute(document.documentElement, 'data-theme', this.currentTheme);
  }
}

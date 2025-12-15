import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';

export type MaterialThemeKey =
  | 'azure-blue'
  | 'rose-red'
  | 'magenta-violet'
  | 'cyan-orange'
  | 'deep-purple-amber'
  | 'indigo-pink'
  | 'pink-blue-grey'
  | 'purple-green';
export type ThemeMode = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly themeLinkId = 'material-dynamic-theme';
  private readonly storageKey = 'material-theme';
  private readonly modeStorageKey = 'material-theme-mode';
  private readonly defaultTheme: MaterialThemeKey = 'indigo-pink';
  private readonly defaultMode: ThemeMode = 'light';
  // SCSS-driven themes keyed by name; CSS is generated in styles.scss and activated via data attributes.
  private readonly themeMap: Record<MaterialThemeKey, true> = {
    'azure-blue': true,
    'rose-red': true,
    'magenta-violet': true,
    'cyan-orange': true,
    'deep-purple-amber': true,
    'indigo-pink': true,
    'pink-blue-grey': true,
    'purple-green': true
  };

  constructor(@Inject(DOCUMENT) private document: Document) { }

  initTheme(): void {
    const saved = (sessionStorage.getItem(this.storageKey) as MaterialThemeKey) || this.defaultTheme;
    const savedMode = (sessionStorage.getItem(this.modeStorageKey) as ThemeMode) || this.defaultMode;
    this.applyTheme(saved, savedMode);
  }

  applyTheme(theme: MaterialThemeKey, mode?: ThemeMode): void {
    const themeKey = this.themeMap[theme] ? theme : this.defaultTheme;
    const modeKey: ThemeMode = mode || this.getCurrentMode();
    sessionStorage.setItem(this.storageKey, themeKey);
    sessionStorage.setItem(this.modeStorageKey, modeKey);
    this.document.body.setAttribute('data-theme', themeKey);
    this.document.body.setAttribute('data-theme-mode', modeKey);
  }

  resetToDefault(): void {
    this.applyTheme(this.defaultTheme, this.defaultMode);
  }

  getCurrentTheme(): MaterialThemeKey {
    return (sessionStorage.getItem(this.storageKey) as MaterialThemeKey) || this.defaultTheme;
  }

  getAvailableThemes(): MaterialThemeKey[] {
    return Object.keys(this.themeMap) as MaterialThemeKey[];
  }

  applyMode(mode: ThemeMode): void {
    const targetMode: ThemeMode = mode || this.defaultMode;
    const currentTheme = this.getCurrentTheme();
    this.applyTheme(currentTheme, targetMode);
  }

  getCurrentMode(): ThemeMode {
    return (sessionStorage.getItem(this.modeStorageKey) as ThemeMode) || this.defaultMode;
  }
}

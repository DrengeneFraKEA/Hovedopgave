/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      colors: {
        primary: 'var(--color-primary)',
        secondary: 'var(--color-secondary)',
        background: 'var(--color-background)',
        text: 'var(--color-text)',
        button: 'var(--color-button)',
        'button-text': 'var(--color-button-text)',
        'button-hover': 'var(--color-button-hover)',
        'box-background': 'var(--color-box-background)',
        'box-border': 'var(--color-box-border)',
      },
    },
  },
  plugins: [require('daisyui')],
  daisyui: {
    themes: ["light", "dark"],
  },
}

/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: 'class',
    content: [
        "./Views/**/*.cshtml",
        "./Views/**/*.razor",
        "./Pages/**/*.cshtml",
        "./wwwroot/js/**/*.js"
    ],
    theme: {
        extend: {},
    },
    plugins: [],
};

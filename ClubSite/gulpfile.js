const gulp = require('gulp');
const sass = require('gulp-sass')(require('sass'));
const postcss = require('gulp-postcss');
const cssnano = require('cssnano');
const autoprefixer = require('autoprefixer');
const rename = require('gulp-rename');
const path = require('path');

// Configure sass options: resolve imports from node_modules and silence dependency deprecation warnings
const sassOptions = {
    // allow @use/@import to resolve packages without relative paths
    loadPaths: [path.join(__dirname, 'node_modules')], // absolute path to node_modules is more reliable
    quietDeps: true,                // suppress deprecation warnings coming from dependency packages (Dart Sass option)
    outputStyle: 'expanded'         // alternative: 'compressed' (we handle minification separately)
};

gulp.task('min',
    function(done) {
        gulp.src('assets/scss/style.scss')
            .pipe(sass(sassOptions).on('error', sass.logError))
            .pipe(postcss([autoprefixer(), cssnano()]))
            .pipe(rename({
                suffix: '.min'
            }))
            .pipe(gulp.dest('wwwroot/assets/css'));
        gulp.src('assets/scss/bootstrap-custom.scss')
            .pipe(sass(sassOptions).on('error', sass.logError))
            .pipe(postcss([autoprefixer(), cssnano()])) // autoprefix + minify
            .pipe(rename({
                basename: 'bootstrap',
                suffix: '.min'
            }))
            .pipe(gulp.dest('wwwroot/assets/css'));
        gulp.src('assets/scss/fontawesome-custom.scss')
            .pipe(sass(sassOptions).on('error', sass.logError))
            .pipe(postcss([autoprefixer(), cssnano()])) // autoprefix + minify
            .pipe(rename({
                basename: 'fontawesome',
                suffix: '.min'
            }))
            .pipe(gulp.dest('wwwroot/assets/css/fontawesome'));
        gulp.src('node_modules/@fortawesome/fontawesome-free/webfonts/*')
            .pipe(gulp.dest('wwwroot/assets/css/fontawesome/webfonts'));
        gulp.src('assets/fonts/fonts-lato-raleway.css')
            .pipe(gulp.dest('wwwroot/assets/fonts/'));
        gulp.src('assets/fonts/lato/*.woff2')
            .pipe(gulp.dest('wwwroot/assets/fonts/lato/'));  
        gulp.src('assets/fonts/raleway/*.woff2')
            .pipe(gulp.dest('wwwroot/assets/fonts/raleway/'));
        gulp.src('node_modules/bootstrap/dist/js/bootstrap.bundle.min.js')
            .pipe(gulp.dest('wwwroot/assets/js/bootstrap/'));
        done();
    });

gulp.task('serve', gulp.parallel(['min']));
gulp.task('default', gulp.series('serve'));

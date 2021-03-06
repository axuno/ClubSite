var gulp = require("gulp"),
    sass = require("gulp-dart-sass");
cssmin = require("gulp-cssmin");
rename = require("gulp-rename");

gulp.task("min",
    function(done) {
        gulp.src("assets/scss/style.scss")
            .pipe(sass().on("error", sass.logError))
            .pipe(cssmin())
            .pipe(rename({
                suffix: ".min"
            }))
            .pipe(gulp.dest("wwwroot/assets/css"));
        gulp.src("assets/scss/bootstrap-custom.scss")
            .pipe(sass().on("error", sass.logError))
            .pipe(cssmin())
            .pipe(rename({
                basename: "bootstrap",
                suffix: ".min"
            }))
            .pipe(gulp.dest("wwwroot/assets/css"));
        gulp.src("assets/scss/fontawesome-custom.scss")
            .pipe(sass().on("error", sass.logError))
            .pipe(cssmin())
            .pipe(rename({
                basename: "fontawesome",
                suffix: ".min"
            }))
            .pipe(gulp.dest("wwwroot/assets/css/fontawesome"));
        gulp.src("node_modules/@fortawesome/fontawesome-free/webfonts/*")
            .pipe(gulp.dest("wwwroot/assets/css/fontawesome/webfonts"));  
        done();
    });

gulp.task("serve", gulp.parallel(["min"]));
gulp.task("default", gulp.series("serve"));
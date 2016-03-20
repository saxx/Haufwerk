var gulp = require("gulp"),
    concat = require("gulp-concat"),

    cssnano = require("gulp-cssnano"),
    sass = require("gulp-sass"),
    prefixer = require("gulp-autoprefixer"),

    uglify = require("gulp-uglify"),
    ts = require("gulp-typescript");

gulp.task("js", function () {
    return gulp.src("./wwwroot/js/**/*.ts")
        .pipe(ts({ module: "commonjs" }))
        .pipe(concat("./wwwroot/js/site.min.js"))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("css", function () {
    return gulp.src(["./wwwroot/css/**/*.scss"])
        .pipe(sass().on("error", sass.logError))
        .pipe(concat("./wwwroot/css/site.min.css"))
        .pipe(prefixer({ browsers: ["last 3 versions"] }))
        .pipe(cssnano())
        .pipe(gulp.dest("."));
});

gulp.task("watch", function () {
    gulp.watch("./wwwroot/css/**/*.scss", ["css"]);
    gulp.watch("./wwwroot/js/**/*.ts", ["js"]);
});

gulp.task("default", ["js", "css", "watch"]);

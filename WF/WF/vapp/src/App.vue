<template>
  <div>
    <header class="container">
      <nav class="navbar navbar-expand-lg navbar-light bg-light">
        <a class="navbar-brand" href="/">敏捷流程系统</a>
        <button
          class="navbar-toggler"
          type="button"
          data-toggle="collapse"
          data-target="#navbarSupportedContent"
          aria-controls="navbarSupportedContent"
          aria-expanded="false"
          aria-label="Toggle navigation"
        >
          <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse" id="navbarSupportedContent">
          <ul class="navbar-nav mr-auto">
            <li class="nav-item active">
              <router-link to="/" class="nav-link">
                待办
                <span class="sr-only">(current)</span>
              </router-link>
            </li>

            <li class="nav-item">
              <router-link to="/DoneTasks" class="nav-link">
                已办
                <span class="sr-only">(current)</span>
              </router-link>
            </li>
            <li class="nav-item dropdown">
              <a
                class="nav-link dropdown-toggle"
                href="#"
                id="navbarDropdown"
                role="button"
                data-toggle="dropdown"
                aria-haspopup="true"
                aria-expanded="false"
              >新建流程</a>
              <div class="dropdown-menu" aria-labelledby="navbarDropdown"></div>
            </li>
            <li class="nav-item">
              <a class="nav-link disabled" href="#" tabindex="-1" aria-disabled="true">Disabled</a>
            </li>
          </ul>
          <form class="form-inline my-2 my-lg-0">
            <input
              class="form-control mr-sm-2"
              type="search"
              placeholder="Search"
              aria-label="Search"
            />
            <button class="btn btn-outline-success my-2 my-sm-0" type="submit">Search</button>
          </form>
        </div>
      </nav>
    </header>
    <div class="container">
      <main role="main" class="pb-3">
        <router-view />
      </main>
    </div>

    <footer class="border-top footer text-muted">
      <div class="container">
        &copy; 2020 - WF.WebApp -
        <a asp-area asp-page="/Privacy">Privacy</a>
      </div>
    </footer>
  </div>
</template>

<script lang="ts">
import { Vue } from "vue-class-component";

export default class App extends Vue {}
</script>
<style>
</style>

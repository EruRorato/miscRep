
angular.module('SteamAppChoser', ['ngMaterial'])
  .controller('gridListDemoCtrl', function ($scope, $http, $mdDialog, $timeout, $mdSidenav, $mdToast) {

      this.tiles = buildGridModel({
          icon: "avatar:svg-",
          title: "Svg-",
          background: "",
          genre: "Genre: ",
          release_date: "Release Date: ",
          link: "http://store.steampowered.com/app/",
          publisher: "Publisher: ",
          mc: "MC: ",
          desc: ""
      });

      function buildGridModel(tileTmpl) {
          var it, results = [];
          $.ajax({
              url: '/home/getGamedata',
              type: 'GET',
              dataType: 'json',
              cache: false,
              async: false,
              success: function (gameInfo) {

                  for (var j = 0; j < 6; j++) {

                      it = angular.extend({}, tileTmpl);
                      //Get from AJAX
                      it.icon = gameInfo[j]["ImageUrl"];
                      it.title = gameInfo[j]["Name"];
                      it.genre = it.genre + (gameInfo[j]["Genre"].length>2 ? gameInfo[j]["Genre"] : "N/A");
                      it.release_date = it.release_date + (gameInfo[j]["ReleaseDate"].length>2 ? gameInfo[j]["ReleaseDate"] : "N/A");
                      it.link = it.link + gameInfo[j]["ImageUrl"].substring(gameInfo[j]["ImageUrl"].lastIndexOf("/steam/apps/") + 12, gameInfo[j]["ImageUrl"].lastIndexOf("/header"));
                      it.publisher = it.publisher + (gameInfo[j]["Publisher"].length > 2 ? gameInfo[j]["Publisher"] : "N/A");
                      it.mc = it.mc + (gameInfo[j]["MetaCritics"].length > 2 ? gameInfo[j]["MetaCritics"] : "N/A");
                      it.desc = (gameInfo[j]["Description"].length > 2 ? gameInfo[j]["Description"] : "There are no description for this game");

                      it.span = { row: 1, col: 1 };

                      it.background = "deepBlue";
                      //switch (j + 1) {
                      //    case 1:
                      //        it.background = "red";
                      //        break;

                      //    case 2: it.background = "green"; break;
                      //    case 3: it.background = "darkBlue"; break;
                      //    case 4:
                      //        it.background = "blue";
                      //        break;

                      //    case 5:
                      //        it.background = "yellow";
                      //        break;

                      //    case 6: it.background = "pink"; break;
                      //    case 7: it.background = "darkBlue"; break;
                      //    case 8: it.background = "purple"; break;
                      //    case 9: it.background = "deepBlue"; break;
                      //    case 10: it.background = "lightPurple"; break;
                      //    case 11: it.background = "yellow"; break;
                      //}

                      results.push(it);
                  }
              }
          });
          
          return results;
      }

      $scope.showAlert = function (ev, link, desc) {
          $mdDialog.show(
            $mdDialog.alert()
              .title('Visit steam page')
              .content(desc)
              .ok("Let's go!")
              .targetEvent(ev)
          ).finally(function () { window.open(link); });
      };


      function newAlert(ev) {
          $mdDialog.show(
            $mdDialog.alert()
              .title('Un-favorite')
              .content('This game no more one of your favorites!')
              .ok("OK")
              .targetEvent(ev)
          );
      };
      $scope.showActionToast = function (ev) {
          var pinTo = 'top right';
          var toast = $mdToast.simple()
            .textContent('Marked as favorite')
            .action('UNDO')
            .highlightAction(true)
            .highlightClass('md-accent')// Accent is used by default, this just demonstrates the usage.
            .position(pinTo);
          $mdToast.show(toast).then(function (response) {
              if (response == 'ok') {
                  newAlert(ev);
              }
          });
      };
  })
.controller('SideCtrl', function ($scope, $timeout, $mdSidenav) {
    $scope.toggleLeft = buildToggler('left');
    $scope.toggleRight = buildToggler('right');
    function buildToggler(componentId) {
        return function () {
            $mdSidenav(componentId).toggle();
        }
    }
})


.controller('CardMenuCtrl', function DemoCtrl($mdDialog) {
    var originatorEv;

    this.menuHref = "http://www.google.com/design/spec/components/menus.html#menus-specs";

    this.openMenu = function ($mdOpenMenu, ev) {
        originatorEv = ev;
        $mdOpenMenu(ev);
    };

    this.announceClick = function (index) {
        $mdDialog.show(
          $mdDialog.alert()
            .title('You clicked!')
            .textContent('You clicked the menu item at index ' + index)
            .ok('Nice')
            .targetEvent(originatorEv)
        );
        originatorEv = null;
    };
})
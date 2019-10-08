
namespace JMatch.Constants
{

    public enum UIEVENT
    {
        NONE,

        CLICK_PLAY,
        CLICK_STOP,

        CLICK_MENU,
        CLICK_SHOWLEADER,
        CLICK_SHOWPOWER,
        CLICK_ADD_POWER1,
        CLICK_ADD_POWER2,
        CLICK_ADD_POWER3,
        UPDATE_GAME_UI,
        CLICK_GAMEOVER,
        CLICK_BUY,
        CLICK_USE_POWER_COLOR,
        CLICK_USE_POWER_DIR,
        CLICK_USE_POWERUP,
        EXECUTE_POWERUP,
        SHOW_GREAT_JOB,
        CHECK_SOUND_UI,
        CLICK_TOGGLE_SOUND,
    };

    public enum ANIM
    {
        NONE,
        FADE,
        SLIDE
    };

    public enum AUDIOCLIPS
    {
        UICLICK = 0,
        SELECT_TILE,
        DESTROY_TILE,
        GAMEOVER,
        POWERUP,
        GAMEWIN
    }

}
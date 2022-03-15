public enum EventType
{
    // gameplay related
    BOMB_UPDATE,        // passes int
    UPDATE_TIME,        // passes float
    START_POS,          // passes vector3
    PLANT_FLAG,         // passes vector3[]
    ADD_EMPTY,          // passes gameobject
    REMOVE_FLAG,        // passes gameobject
    ADD_GOOD_TILE,      // passes gameobject
    COUNT_BOMBS,        // passes nothing  
    PICK_TILE,          // passes nothing
    PLAY_CLICK,         // passes nothing
    PLAY_FLAG,          // passes nothing
    TILE_CLICK,         // passes nothing
    REVEAL_TILE,        // passes nothing
    // gameflow related
    PREPARE_GAME,       // passes nothing
    RANDOM_GRID,        // passes nothing
    RESET_GAME,         // passes nothing
    START_GAME,         // passes nothing
    END_GAME,           // passes nothing
    WIN_GAME,           // passes nothing
    GAME_LOSE,          // passes nothing
    UNPLAYABLE,         // passes nothing
    PLAYABLE,           // passes nothing
    // input related
    INPUT_LEFT,         // passes nothing
    INPUT_RIGHT,        // passes nothing
    INPUT_UP,           // passes nothing
    INPUT_SCROLL_UP,    // passes nothing
    INPUT_DOWN,         // passes nothing
    INPUT_SCROLL_DOWN,  // passes nothing
    INPUT_FORWARD,      // passes nothing
    INPUT_BACK,         // passes nothing
    INPUT_FS,           // passes nothing
    INPUT_SPEED,        // passes nothing
    // other stuff
    UPDATE_BGM,         // passes float
    UPDATE_SFX,         // passes float
    UPDATE_SFX_MAIN,    // passes float
    ENABLE_RTX,         // passes nothing
    DISABLE_RTX,        // passes nothing
}

public enum TileStates
{
    Revealed,
    Number,
    Bomb,
    Empty
}

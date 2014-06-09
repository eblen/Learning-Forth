\ Copyright (c) 2014 John Eblen

\ See the file license.txt for copying permission.

\ ================================================================================
\ Constants
\ ================================================================================
15 constant puzzle-size
36 constant num-possible-moves


\ ================================================================================
\ Utility words
\ ================================================================================

: 3drop ( a b c -- )
  2drop drop ;

: 3dup ( a b c -- a b c a b c )
  dup 2over rot ;


\ ================================================================================
\ Move data
\ ================================================================================

\ List of all possible moves
\ format: <peg start> <peg jumped> <peg end>
create move-data 0 , 1 , 3 , 0 , 2 , 5 ,
                 1 , 3 , 6 , 1 , 4 , 8 ,
                 2 , 4 , 7 , 2 , 5 , 9 ,
                 3 , 1 , 0 , 3 , 4 , 5 , 3 , 6 , 10 , 3 , 7 , 12 ,
                 4 , 7 , 11 , 4 , 8 , 13 ,
                 5 , 2 , 0 , 5 , 4 , 3 , 5 , 8 , 12 , 5 , 9 , 14 ,
                 6 , 3 , 1 , 6 , 7 , 8 ,
                 7 , 4 , 2 , 7 , 8 , 9 ,
                 8 , 4 , 1 , 8 , 7 , 6 ,
                 9 , 5 , 2 , 9 , 8 , 7 ,
                 10 , 6 , 3 , 10 , 11 , 12 ,
                 11 , 7 , 4 , 11 , 12 , 13 ,
                 12 , 7 , 3 , 12 , 8 , 5 , 12 , 11 , 10 , 12 , 13 , 14 ,
                 13 , 8 , 4 , 13 , 12 , 11 ,
                 14 , 9 , 5 , 14 , 13 , 12 ,

: get-move-address ( move-num -- addr )
  3 * cells move-data + ;

: get-move-start ( move-num -- position )
  get-move-address @ ;

: get-move-jump ( move-num -- position )
  get-move-address cell+ @ ;

: get-move-end ( move-num -- position )
  get-move-address cell+ cell+ @ ;

: get-move-pos ( move-num -- start jump end)
  get-move-address dup @ swap dup cell+ @ swap cell+ cell+ @ ;

: get-move ( start jump end -- move-num | -1 if not found )
  num-possible-moves 0 do
      3dup I get-move-end = -rot
           I get-move-jump = swap
           I get-move-start =
           and and if 3drop I unloop exit then loop
  3drop -1 ;


\ ================================================================================
\ Basic puzzle words
\ ================================================================================

\ Allocate a new puzzle in the dictionary with the default peg layout
\ Usage: create <puzzle name> new-puzzle
: new-puzzle ( -- )
  0 , puzzle-size 1 do 1 , loop ;

: place-peg ( puzzle pos -- )
  cells + true swap ! ;

: remove-peg ( puzzle pos -- )
  cells + false swap ! ;

: check-peg ( puzzle pos -- <whether peg occupies given position> )
  cells + @ ;

: count-pegs ( puzzle -- count )
  0 puzzle-size 0 do
      over i check-peg
        if 1+ then loop
  swap drop ;

\ Reset puzzle to the default layout
: reset-puzzle ( puzzle -- )
  dup 0 remove-peg
  puzzle-size 1 do dup i place-peg loop drop ;


\ ================================================================================
\ Game-level puzzle words
\ ================================================================================

: valid-move ( puzzle move-num -- <whether move is possible> )
  2dup 2dup get-move-start check-peg -rot
            get-move-jump  check-peg and -rot
            get-move-end   check-peg 0= and ;

: make-move ( puzzle move-num -- )
  2dup valid-move if
      2dup 2dup get-move-start remove-peg
                get-move-jump  remove-peg
                get-move-end   place-peg then ;

: rev-move ( puzzle move-num -- )
    2dup 2dup get-move-start place-peg
              get-move-jump  place-peg
              get-move-end   remove-peg ;
  
: win ( puzzle -- boolean )
  count-pegs 1 = ;

: print-puzzle ( puzzle -- )
  puzzle-size 0 do dup i cells + @ . loop drop ;

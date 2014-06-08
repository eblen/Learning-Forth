#! /opt/local/bin/gforth

s" /Users/john/forth/lo-puzzle-board.fs" included

\ ================================================================================
\ Utilities
\ ================================================================================

: 3rev ( a b c -- c b a )
  -rot swap ;

: inc-var ( a-addr -- )
  dup @ 1+ swap ! ;

: dec-var ( a-addr -- )
  dup @ 1- swap ! ;


\ ================================================================================
\ Storage and words for moves in solutions
\ ================================================================================
variable num-moves
0 num-moves !

create move-stack 15 cells allot

: push-move ( move-num -- )
  num-moves @ cells move-stack + !
  num-moves inc-var ;

: pop-move ( -- )
  num-moves dec-var ;

: print-moves ( -- )
  move-stack num-moves @ 0 do
      dup I cells + @ get-move-pos 3rev . . . bl emit bl emit loop
  CR drop ;


\ ================================================================================
\ Solver
\ ================================================================================
: solve ( puzzle -- )
  num-possible-moves 0 do
      dup I valid-move if
          dup I make-move I push-move dup win if print-moves
              else dup recurse then pop-move dup I rev-move then loop
  drop ;


\ ================================================================================
\ High-level commands
\ ================================================================================
create puzzle new-puzzle
puzzle solve
bye
